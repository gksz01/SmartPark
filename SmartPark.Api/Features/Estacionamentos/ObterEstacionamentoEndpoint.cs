using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Estacionamentos.ObterEstacionamento;
using SmartPark.Contracts.Estacionamentos;

namespace SmartPark.Api.Features.Estacionamentos;

public sealed class ObterEstacionamentoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/estacionamentos/{estacionamentoId:guid}", async (Guid estacionamentoId, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ObterEstacionamentoQuery(estacionamentoId), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization()
            .WithTags(Tags.Estacionamentos)
            .WithSummary("Detalhes de um estacionamento.")
            .Produces<EstacionamentoResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
}
