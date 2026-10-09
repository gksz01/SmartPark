using BlazorBlueprint.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SmartPark.Web.Autenticacao;
using SmartPark.Web.Comum;
using SmartPark.Web.Components;
using SmartPark.Web.Features.Administracao;
using SmartPark.Web.Features.Autenticacao;
using SmartPark.Web.Features.Estacionamentos;
using SmartPark.Web.Features.Reservas;
using SmartPark.Web.Features.Veiculos;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBlueprintComponents(configureTheme: theme =>
{
    theme.DefaultBaseColor = BaseColor.Slate;
    theme.DefaultPrimaryColor = PrimaryColor.Blue;
    theme.DetectSystemPreference = true;
});

// Sessão do usuário: o token da API fica no armazenamento protegido do navegador e alimenta o
// AuthenticationStateProvider do circuito.
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<SessaoStore>();
builder.Services.AddScoped<SessaoAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<SessaoAuthenticationStateProvider>());

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<FusoHorario>();
builder.Services.AddScoped<Geolocalizacao>();

// Um cliente HTTP tipado por fatia. "api" é resolvido pelo service discovery do Aspire.
var api = new Uri("https+http://api");
builder.Services.AddHttpClient<AutenticacaoApi>(c => c.BaseAddress = api);
builder.Services.AddHttpClient<VeiculosApi>(c => c.BaseAddress = api);
builder.Services.AddHttpClient<EstacionamentosApi>(c => c.BaseAddress = api);
builder.Services.AddHttpClient<ReservasApi>(c => c.BaseAddress = api);
builder.Services.AddHttpClient<AdministracaoApi>(c => c.BaseAddress = api);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
