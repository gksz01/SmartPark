using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SmartPark.Infrastructure.Persistencia;

namespace SmartPark.Migrations.PostgreSql;

/// <summary>
/// Usada só pelo <c>dotnet ef</c> para gerar e aplicar as migrations do PostgreSQL. Gerar uma migration
/// não abre conexão; a connection string só importa para <c>database update</c>.
/// </summary>
internal sealed class PostgreSqlDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SmartParkDbContext>
{
    public SmartParkDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable($"ConnectionStrings__{BancoDeDados.NomeConexao}")
            ?? "Host=localhost;Port=5432;Database=smartparkdb;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<SmartParkDbContext>();
        BancoDeDados.Configurar(options, ProvedorBancoDados.PostgreSql, connectionString);

        return new SmartParkDbContext(options.Options);
    }
}
