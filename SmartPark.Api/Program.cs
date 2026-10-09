using Scalar.AspNetCore;
using SmartPark.Api.Configuracao;
using SmartPark.Application;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Behaviors;
using SmartPark.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddInfrastructure();
builder.Services.AddApplication();

// O gerador do Mediator lê estas opções em tempo de compilação: os valores precisam ser literais aqui.
builder.Services.AddMediator(options =>
{
    // Scoped porque os handlers dependem do DbContext da requisição.
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.Assemblies = [typeof(SmartPark.Application.AssemblyReference)];
    options.PipelineBehaviors = [typeof(LoggingBehavior<,>), typeof(ValidationBehavior<,>)];
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddPoliticas();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecurityTransformer>());
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("SmartPark API"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();
app.MapDefaultEndpoints();

app.Run();
