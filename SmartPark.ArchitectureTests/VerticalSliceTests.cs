using FluentValidation;
using Mediator;
using SmartPark.Api.Configuracao;

namespace SmartPark.ArchitectureTests;

/// <summary>
/// Convenções do Vertical Slice: cada caso de uso é uma pasta com mensagem, handler e validator juntos,
/// e cada endpoint fica na pasta do seu módulo na API.
/// </summary>
public sealed class VerticalSliceTests
{
    private static readonly Type[] InterfacesDeHandler = [typeof(ICommandHandler<,>), typeof(IQueryHandler<,>)];

    private static readonly string[] NamespacesGenericosProibidos = [".Services", ".Repositories", ".Handlers", ".Dtos", ".Helpers"];

    public static TheoryData<Type> Handlers() =>
        [.. Assemblies.TiposDe(Assemblies.Application).Where(t => !t.IsAbstract && MensagemDoHandler(t) is not null)];

    public static TheoryData<Type> Validators() =>
        [.. Assemblies.TiposDe(Assemblies.Application).Where(t => !t.IsAbstract && MensagemDoValidator(t) is not null)];

    public static TheoryData<Type> Endpoints() =>
        [.. Assemblies.TiposDe(Assemblies.Api).Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IEndpoint)))];

    [Fact]
    public void Existem_handlers_validators_e_endpoints_para_verificar()
    {
        Assert.NotEmpty(Handlers());
        Assert.NotEmpty(Validators());
        Assert.NotEmpty(Endpoints());
    }

    [Theory]
    [MemberData(nameof(Handlers))]
    public void Handler_e_sealed_e_vive_na_pasta_do_seu_caso_de_uso(Type handler)
    {
        var mensagem = MensagemDoHandler(handler)!;

        Assert.True(handler.IsSealed, $"{handler.Name} deve ser sealed.");
        Assert.EndsWith("Handler", handler.Name, StringComparison.Ordinal);
        Assert.StartsWith("SmartPark.Application.Features.", handler.Namespace, StringComparison.Ordinal);
        Assert.Equal(mensagem.Namespace, handler.Namespace);

        // CriarReservaHandler trata CriarReservaCommand ou CriarReservaQuery, e a pasta tem o mesmo nome.
        var casoDeUso = handler.Name[..^"Handler".Length];
        Assert.Contains(mensagem.Name, new[] { casoDeUso + "Command", casoDeUso + "Query" });
        Assert.EndsWith("." + casoDeUso, handler.Namespace, StringComparison.Ordinal);
    }

    [Theory]
    [MemberData(nameof(Validators))]
    public void Validator_e_sealed_e_fica_junto_da_mensagem_que_valida(Type validator)
    {
        var mensagem = MensagemDoValidator(validator)!;

        Assert.True(validator.IsSealed, $"{validator.Name} deve ser sealed.");
        Assert.EndsWith("Validator", validator.Name, StringComparison.Ordinal);
        Assert.Equal(mensagem.Namespace, validator.Namespace);
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void Endpoint_e_sealed_e_fica_na_pasta_do_modulo(Type endpoint)
    {
        Assert.True(endpoint.IsSealed, $"{endpoint.Name} deve ser sealed.");
        Assert.EndsWith("Endpoint", endpoint.Name, StringComparison.Ordinal);
        Assert.StartsWith("SmartPark.Api.Features.", endpoint.Namespace, StringComparison.Ordinal);
    }

    [Fact]
    public void Toda_mensagem_tem_um_endpoint_com_o_mesmo_caso_de_uso()
    {
        var endpoints = Assemblies.TiposDe(Assemblies.Api)
            .Where(t => t.IsAssignableTo(typeof(IEndpoint)))
            .Select(t => t.Name[..^"Endpoint".Length])
            .ToHashSet();

        var semEndpoint = Assemblies.TiposDe(Assemblies.Application)
            .Where(t => !t.IsAbstract && MensagemDoHandler(t) is not null)
            .Select(t => t.Name[..^"Handler".Length])
            .Where(casoDeUso => !endpoints.Contains(casoDeUso))
            .ToList();

        Assert.True(semEndpoint.Count == 0, "Casos de uso sem endpoint: " + string.Join(", ", semEndpoint));
    }

    [Fact]
    public void Nao_existem_pastas_tecnicas_genericas_na_application_e_na_api()
    {
        var proibidos = Assemblies.TiposDe(Assemblies.Application)
            .Concat(Assemblies.TiposDe(Assemblies.Api))
            .Where(t => NamespacesGenericosProibidos.Any(n => t.Namespace!.Contains(n, StringComparison.Ordinal)))
            .Select(t => t.FullName)
            .ToList();

        Assert.True(proibidos.Count == 0, "Organize por feature, não por tipo técnico: " + string.Join(", ", proibidos));
    }

    private static Type? MensagemDoHandler(Type tipo) =>
        tipo.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && InterfacesDeHandler.Contains(i.GetGenericTypeDefinition()))
            ?.GetGenericArguments()[0];

    private static Type? MensagemDoValidator(Type tipo)
    {
        for (var atual = tipo.BaseType; atual is not null; atual = atual.BaseType)
        {
            if (atual.IsGenericType && atual.GetGenericTypeDefinition() == typeof(AbstractValidator<>))
            {
                var alvo = atual.GetGenericArguments()[0];

                // Validators de partes compartilhadas (ex.: dados do estacionamento) não validam mensagens.
                return alvo.IsAssignableTo(typeof(IMessage)) ? alvo : null;
            }
        }

        return null;
    }
}
