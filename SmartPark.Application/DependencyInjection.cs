using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SmartPark.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra o que a camada de aplicação precisa. O Mediator é registrado no host, porque o gerador
    /// de código dele só pode existir em um projeto por aplicação.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly, includeInternalTypes: true);
        services.TryAddSingleton(TimeProvider.System);

        return services;
    }
}
