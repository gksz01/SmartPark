using Microsoft.EntityFrameworkCore;

namespace SmartPark.Infrastructure.Persistencia;

/// <summary>
/// Configuração do provider compartilhada entre a aplicação e as factories de design-time, para que as
/// migrations de cada provider sejam geradas exatamente com o modelo que roda.
/// </summary>
public static class BancoDeDados
{
    /// <summary>Nome do recurso de banco no AppHost e da connection string.</summary>
    public const string NomeConexao = "smartparkdb";

    public const string SecaoProvider = "Database:Provider";

    public const string AssemblyMigrationsPostgreSql = "SmartPark.Migrations.PostgreSql";

    public const string AssemblyMigrationsSqlServer = "SmartPark.Migrations.SqlServer";

    public static DbContextOptionsBuilder Configurar(
        DbContextOptionsBuilder options,
        ProvedorBancoDados provedor,
        string? connectionString)
    {
        _ = provedor switch
        {
            ProvedorBancoDados.PostgreSql => options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(AssemblyMigrationsPostgreSql)),
            ProvedorBancoDados.SqlServer => options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsAssembly(AssemblyMigrationsSqlServer)),
            _ => throw new ArgumentOutOfRangeException(nameof(provedor), provedor, "Provider de banco não suportado.")
        };

        return options.UseSnakeCaseNamingConvention();
    }
}
