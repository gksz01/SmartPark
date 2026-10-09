using OpenTelemetry.Trace;
using SmartPark.Infrastructure;
using SmartPark.MigrationService;
using SmartPark.MigrationService.Seed;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddPersistencia();
builder.Services.AddIdentidade();

builder.Services.AddOptions<SeedOptions>().Bind(builder.Configuration.GetSection(SeedOptions.Secao));
builder.Services.AddScoped<SeedDados>();
builder.Services.AddHostedService<MigracaoWorker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(MigracaoWorker.ActivitySourceName));

builder.Build().Run();
