using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Disponibilidade.ConsultarDisponibilidade;
using SmartPark.Contracts.Estacionamentos;

namespace SmartPark.Api.Features.Disponibilidade;

public sealed class ConsultarDisponibilidadeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/estacionamentos/{estacionamentoId:guid}/disponibilidade", async (
                Guid estacionamentoId,
                DateTimeOffset inicio,
                DateTimeOffset fim,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ConsultarDisponibilidadeQuery(estacionamentoId, inicio, fim), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization()
            .WithTags(Tags.Disponibilidade)
            .WithSummary("Capacidade reservável livre no período informado (RF05).")
            .Produces<DisponibilidadeResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
}
