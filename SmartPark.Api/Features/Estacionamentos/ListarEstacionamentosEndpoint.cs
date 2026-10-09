using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Estacionamentos.ListarEstacionamentos;
using SmartPark.Contracts.Estacionamentos;

namespace SmartPark.Api.Features.Estacionamentos;

public sealed class ListarEstacionamentosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/estacionamentos", async (
                string? busca,
                double? latitude,
                double? longitude,
                bool? incluirInativos,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(
                    new ListarEstacionamentosQuery(busca, latitude, longitude, incluirInativos ?? false),
                    cancellationToken);

                return resultado.ToOk();
            })
            .RequireAuthorization()
            .WithTags(Tags.Estacionamentos)
            .WithSummary("Localiza estacionamentos por texto e, com a posição do motorista, por distância (RF04).")
            .Produces<IReadOnlyList<EstacionamentoResponse>>();
}
