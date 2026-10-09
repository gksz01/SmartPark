using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Application.Features.Autenticacao.RegistrarMotorista;

public sealed class RegistrarMotoristaHandler(
    IApplicationDbContext db,
    IIdentityService identityService,
    ITokenService tokenService)
    : ICommandHandler<RegistrarMotoristaCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(RegistrarMotoristaCommand command, CancellationToken cancellationToken)
    {
        var email = Usuario.NormalizarEmail(command.Email);
        if (await db.Usuarios.AnyAsync(u => u.Email == email, cancellationToken))
        {
            return UsuarioErrors.EmailEmUso;
        }

        var usuario = Usuario.Criar(command.Nome, email, Perfil.Motorista);
        db.Usuarios.Add(usuario);

        var credenciais = await identityService.CriarCredenciaisAsync(usuario.Id, email, command.Senha, cancellationToken);
        if (credenciais.IsFailure)
        {
            return credenciais.Error;
        }

        // Cadastro já entra logado.
        return tokenService.Gerar(usuario).ParaTokenResponse(usuario);
    }
}
