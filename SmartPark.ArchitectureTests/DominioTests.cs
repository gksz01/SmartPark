using System.Reflection;
using SmartPark.Domain.Common;

namespace SmartPark.ArchitectureTests;

/// <summary>Encapsulamento do domínio: estado só muda pelos métodos das entidades.</summary>
public sealed class DominioTests
{
    public static TheoryData<Type> Entidades() =>
        [.. Assemblies.TiposDe(Assemblies.Domain).Where(t => t.IsSubclassOf(typeof(Entity)) && !t.IsAbstract)];

    [Theory]
    [MemberData(nameof(Entidades))]
    public void Entidade_e_sealed_e_nao_tem_setters_publicos(Type entidade)
    {
        Assert.True(entidade.IsSealed, $"{entidade.Name} deve ser sealed.");

        var settersPublicos = entidade
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.SetMethod?.IsPublic == true)
            .Select(p => p.Name)
            .ToList();

        Assert.True(settersPublicos.Count == 0, $"{entidade.Name} expõe setters públicos: {string.Join(", ", settersPublicos)}");
    }

    [Theory]
    [MemberData(nameof(Entidades))]
    public void Entidade_nao_tem_construtor_publico(Type entidade)
    {
        // A criação passa por métodos de fábrica que validam as invariantes.
        Assert.Empty(entidade.GetConstructors(BindingFlags.Public | BindingFlags.Instance));
    }
}
