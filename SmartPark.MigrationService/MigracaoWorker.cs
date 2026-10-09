using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SmartPark.Infrastructure.Persistencia;
using SmartPark.MigrationService.Seed;

namespace SmartPark.MigrationService;

/// <summary>
/// Aplica as migrations do provider configurado, roda o seed e encerra. O AppHost só sobe a API depois
/// que este processo termina com sucesso (WaitForCompletion).
/// </summary>
internal sealed partial class MigracaoWorker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime lifetime,
    ILogger<MigracaoWorker> logger) : BackgroundService
{
    public const string ActivitySourceName = "SmartPark.Migracoes";

    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrar banco de dados", ActivityKind.Client);

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<SmartParkDbContext>();

            var estrategia = db.Database.CreateExecutionStrategy();
            await estrategia.ExecuteAsync(async () => await db.Database.MigrateAsync(stoppingToken));
            LogMigrado(logger, db.Database.ProviderName ?? "?");

            await scope.ServiceProvider.GetRequiredService<SeedDados>().ExecutarAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            LogFalha(logger, ex);

            // Código de saída diferente de zero impede a API de subir sobre um banco desatualizado.
            Environment.ExitCode = 1;
        }

        lifetime.StopApplication();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Migrations aplicadas ({Provider})")]
    private static partial void LogMigrado(ILogger logger, string provider);

    [LoggerMessage(Level = LogLevel.Critical, Message = "Falha ao migrar o banco de dados")]
    private static partial void LogFalha(ILogger logger, Exception exception);
}
