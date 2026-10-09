namespace SmartPark.Api.Configuracao;

/// <summary>
/// Um endpoint HTTP de uma fatia. Cada caso de uso tem o seu, descoberto por reflexão na inicialização,
/// então adicionar um caso de uso não exige tocar em nenhum arquivo central.
/// </summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
