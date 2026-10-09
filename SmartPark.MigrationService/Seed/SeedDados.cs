using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Domain.Estacionamentos;
using SmartPark.Domain.Usuarios;
using SmartPark.Infrastructure.Persistencia;

namespace SmartPark.MigrationService.Seed;

/// <summary>Seed idempotente: só cria o que ainda não existe, então pode rodar a cada subida.</summary>
internal sealed partial class SeedDados(
    SmartParkDbContext db,
    IIdentityService identityService,
    IOptions<SeedOptions> options,
    ILogger<SeedDados> logger)
{
    private const string EmailMotoristaDemo = "motorista@smartpark.local";

    public async Task ExecutarAsync(CancellationToken cancellationToken)
    {
        var seed = options.Value;

        if (string.IsNullOrWhiteSpace(seed.AdminPassword))
        {
            LogSemSenhaAdmin(logger);
        }
        else
        {
            await GarantirUsuarioAsync(seed.AdminNome, seed.AdminEmail, seed.AdminPassword, Perfil.Administrador, cancellationToken);

            if (seed.DadosDemo)
            {
                await GarantirUsuarioAsync("Motorista Demo", EmailMotoristaDemo, seed.AdminPassword, Perfil.Motorista, cancellationToken);
            }
        }

        if (seed.DadosDemo)
        {
            await GarantirEstacionamentosDemoAsync(cancellationToken);
        }
    }

    private async Task GarantirUsuarioAsync(string nome, string email, string senha, Perfil perfil, CancellationToken cancellationToken)
    {
        email = Usuario.NormalizarEmail(email);
        if (await db.Usuarios.AnyAsync(u => u.Email == email, cancellationToken))
        {
            return;
        }

        var usuario = Usuario.Criar(nome, email, perfil);
        db.Usuarios.Add(usuario);

        var resultado = await identityService.CriarCredenciaisAsync(usuario.Id, email, senha, cancellationToken);
        if (resultado.IsFailure)
        {
            db.ChangeTracker.Clear();
            throw new InvalidOperationException($"Não foi possível criar o usuário {email}: {resultado.Error.Message}");
        }

        LogUsuarioCriado(logger, email, perfil);
    }

    private async Task GarantirEstacionamentosDemoAsync(CancellationToken cancellationToken)
    {
        if (await db.Estacionamentos.AnyAsync(cancellationToken))
        {
            return;
        }

        (string Nome, string Endereco, double Lat, double Lng, int Total, int Reservavel, decimal ValorHora, (string Prefixo, int Quantidade)[] Setores)[] demos =
        [
            ("SmartPark Centro", "Rua XV de Novembro, 1000 - Centro, Curitiba - PR", -25.4296, -49.2713, 40, 30, 12m, [("A", 20), ("B", 20)]),
            ("SmartPark PUCPR", "Rua Imaculada Conceição, 1155 - Prado Velho, Curitiba - PR", -25.4518, -49.2512, 60, 40, 8m, [("P", 60)]),
            ("SmartPark Batel", "Av. do Batel, 1868 - Batel, Curitiba - PR", -25.4422, -49.2900, 25, 15, 15m, [("B", 25)])
        ];

        foreach (var demo in demos)
        {
            var estacionamento = Estacionamento.Criar(demo.Nome, demo.Endereco, demo.Lat, demo.Lng, demo.Total, demo.Reservavel, demo.ValorHora).Value;

            foreach (var (prefixo, quantidade) in demo.Setores)
            {
                estacionamento.AdicionarVagas(prefixo, quantidade);
            }

            db.Estacionamentos.Add(estacionamento);
        }

        await db.SaveChangesAsync(cancellationToken);
        LogDemosCriados(logger, demos.Length);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Seed:AdminPassword não configurada; o administrador inicial não foi criado")]
    private static partial void LogSemSenhaAdmin(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Usuário {Email} criado com perfil {Perfil}")]
    private static partial void LogUsuarioCriado(ILogger logger, string email, Perfil perfil);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Quantidade} estacionamentos de demonstração criados")]
    private static partial void LogDemosCriados(ILogger logger, int quantidade);
}
