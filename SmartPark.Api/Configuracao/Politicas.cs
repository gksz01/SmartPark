using SmartPark.Contracts.Autenticacao;

namespace SmartPark.Api.Configuracao;

/// <summary>Políticas de autorização por perfil (RO01).</summary>
public static class Politicas
{
    public const string Motorista = nameof(Motorista);
    public const string Administrador = nameof(Administrador);

    public static IServiceCollection AddPoliticas(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Motorista, p => p.RequireRole(Perfis.Motorista))
            .AddPolicy(Administrador, p => p.RequireRole(Perfis.Administrador));

        return services;
    }
}
