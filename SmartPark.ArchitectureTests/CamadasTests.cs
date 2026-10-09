using NetArchTest.Rules;

namespace SmartPark.ArchitectureTests;

/// <summary>Regra de dependência da Clean Architecture: as dependências só apontam para dentro.</summary>
public sealed class CamadasTests
{
    [Fact]
    public void Domain_nao_depende_de_nenhuma_outra_camada_nem_de_frameworks()
    {
        var resultado = Types.InAssembly(Assemblies.Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "SmartPark.Contracts",
                "SmartPark.Application",
                "SmartPark.Infrastructure",
                "SmartPark.Api",
                "SmartPark.Web",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Mediator",
                "FluentValidation")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Falhas(resultado));
    }

    [Fact]
    public void Contracts_nao_depende_de_nenhum_outro_projeto()
    {
        var resultado = Types.InAssembly(Assemblies.Contracts)
            .ShouldNot()
            .HaveDependencyOnAny(
                "SmartPark.Domain",
                "SmartPark.Application",
                "SmartPark.Infrastructure",
                "SmartPark.Api",
                "SmartPark.Web")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Falhas(resultado));
    }

    [Fact]
    public void Application_nao_depende_de_infraestrutura_host_nem_provider_de_banco()
    {
        var resultado = Types.InAssembly(Assemblies.Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "SmartPark.Infrastructure",
                "SmartPark.Api",
                "SmartPark.Web",
                "Microsoft.AspNetCore",
                "Npgsql",
                "Microsoft.EntityFrameworkCore.SqlServer",
                "Microsoft.Data.SqlClient")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Falhas(resultado));
    }

    [Fact]
    public void Infrastructure_nao_depende_dos_hosts()
    {
        var resultado = Types.InAssembly(Assemblies.Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny("SmartPark.Api", "SmartPark.Web")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Falhas(resultado));
    }

    [Fact]
    public void Web_fala_com_o_backend_somente_pelos_contratos_http()
    {
        var resultado = Types.InAssembly(Assemblies.Web)
            .ShouldNot()
            .HaveDependencyOnAny(
                "SmartPark.Domain",
                "SmartPark.Application",
                "SmartPark.Infrastructure",
                "SmartPark.Api",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Falhas(resultado));
    }

    [Fact]
    public void Api_nao_acessa_o_banco_diretamente()
    {
        // Endpoints só traduzem HTTP e enviam mensagens ao Mediator; dados são assunto dos handlers.
        var resultado = Types.InAssembly(Assemblies.Api)
            .That().ResideInNamespaceStartingWith("SmartPark.Api.Features")
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "SmartPark.Infrastructure.Persistencia", "SmartPark.Domain")
            .GetResult();

        Assert.True(resultado.IsSuccessful, Falhas(resultado));
    }

    private static string Falhas(NetArchTest.Rules.TestResult resultado) =>
        "Tipos que violam a regra: " + string.Join(", ", resultado.FailingTypeNames ?? []);
}
