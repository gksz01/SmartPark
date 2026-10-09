using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SmartPark.Infrastructure.Persistencia;

namespace SmartPark.Migrations.SqlServer;

/// <summary>
/// Usada só pelo <c>dotnet ef</c> para gerar e aplicar as migrations do SQL Server. Gerar uma migration
/// não abre conexão; a connection string só importa para <c>database update</c>.
/// </summary>
internal sealed class SqlServerDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SmartParkDbContext>
{
    public SmartParkDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable($"ConnectionStrings__{BancoDeDados.NomeConexao}")
            ?? "Server=localhost,1433;Database=smartparkdb;User Id=sa;Password=SmartPark@Dev1;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<SmartParkDbContext>();
        BancoDeDados.Configurar(options, ProvedorBancoDados.SqlServer, connectionString);

        return new SmartParkDbContext(options.Options);
    }
}
