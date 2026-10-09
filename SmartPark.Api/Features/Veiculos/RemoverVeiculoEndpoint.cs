using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Veiculos.RemoverVeiculo;

namespace SmartPark.Api.Features.Veiculos;

public sealed class RemoverVeiculoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("/api/veiculos/{veiculoId:guid}", async (Guid veiculoId, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new RemoverVeiculoCommand(veiculoId), cancellationToken);
                return resultado.ToNoContent();
            })
            .RequireAuthorization(Politicas.Motorista)
            .WithTags(Tags.Veiculos)
            .WithSummary("Remove (desativa) um veículo sem reservas ativas.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
