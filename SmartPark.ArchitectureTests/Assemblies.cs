using System.Reflection;

namespace SmartPark.ArchitectureTests;

internal static class Assemblies
{
    public static readonly Assembly Domain = typeof(Domain.Common.Entity).Assembly;
    public static readonly Assembly Contracts = typeof(Contracts.Autenticacao.Perfis).Assembly;
    public static readonly Assembly Application = typeof(Application.AssemblyReference).Assembly;
    public static readonly Assembly Infrastructure = typeof(Infrastructure.DependencyInjection).Assembly;
    public static readonly Assembly Api = typeof(Api.Configuracao.IEndpoint).Assembly;
    public static readonly Assembly Web = typeof(Web.Comum.Formatos).Assembly;

    /// <summary>Tipos escritos à mão (sem os gerados pelo compilador, como closures e state machines).</summary>
    public static IEnumerable<Type> TiposDe(Assembly assembly) =>
        assembly.GetTypes().Where(t =>
            !t.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false)
            && !t.Name.Contains('<', StringComparison.Ordinal)
            && t.Namespace?.StartsWith("SmartPark", StringComparison.Ordinal) == true);
}
