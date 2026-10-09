var builder = DistributedApplication.CreateBuilder(args);

// Segredos como parâmetros do Aspire: em desenvolvimento vêm dos user-secrets deste projeto
// (Parameters:jwt-key e Parameters:admin-password); se faltarem, o dashboard pede os valores.
var jwtKey = builder.AddParameter("jwt-key", secret: true);
var adminPassword = builder.AddParameter("admin-password", secret: true);

var isPublish = builder.ExecutionContext.IsPublishMode;

// --- Banco ---------------------------------------------------------------------------------------
// Desenvolvimento: PostgreSQL em contêiner. Para exercitar o provider dos demais ambientes na própria
// máquina, suba com SMARTPARK_DB_PROVIDER=SqlServer. Em publish, o SQL Server já existente chega por
// connection string. O recurso se chama "smartparkdb" nos três casos, então nada abaixo muda.
var useSqlServer = isPublish || string.Equals(
    Environment.GetEnvironmentVariable("SMARTPARK_DB_PROVIDER"),
    "SqlServer",
    StringComparison.OrdinalIgnoreCase);

var provider = useSqlServer ? "SqlServer" : "PostgreSql";

IResourceBuilder<IResourceWithConnectionString> database;

if (isPublish)
{
    database = builder.AddConnectionString("smartparkdb");
}
else if (useSqlServer)
{
    database = builder.AddSqlServer("sqlserver")
        .WithDataVolume()
        .AddDatabase("smartparkdb");
}
else
{
    database = builder.AddPostgres("postgres")
        .WithPgAdmin()
        .WithDataVolume()
        .AddDatabase("smartparkdb");
}

// --- Migrations + seed ---------------------------------------------------------------------------
var migrations = builder.AddProject<Projects.SmartPark_MigrationService>("migrations")
    .WithReference(database)
    .WaitFor(database)
    .WithEnvironment("Database__Provider", provider)
    .WithEnvironment("Seed__AdminPassword", adminPassword)
    // Dados de demonstração só no desenvolvimento.
    .WithEnvironment("Seed__DadosDemo", isPublish ? "false" : "true");

// --- API -----------------------------------------------------------------------------------------
var api = builder.AddProject<Projects.SmartPark_Api>("api")
    .WithReference(database)
    .WaitForCompletion(migrations)
    .WithEnvironment("Database__Provider", provider)
    .WithEnvironment("Jwt__Key", jwtKey);

// --- Front-end -----------------------------------------------------------------------------------
builder.AddProject<Projects.SmartPark_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
