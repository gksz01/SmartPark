using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Veiculos.ListarMeusVeiculos;
using SmartPark.Contracts.Veiculos;

namespace SmartPark.Api.Features.Veiculos;

public sealed class ListarMeusVeiculosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/veiculos", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ListarMeusVeiculosQuery(), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization(Politicas.Motorista)
            .WithTags(Tags.Veiculos)
            .WithSummary("Lista os veículos ativos do motorista.")
            .Produces<IReadOnlyList<VeiculoResponse>>();
}
