using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Vagas.RemoverVaga;

namespace SmartPark.Api.Features.Vagas;

public sealed class RemoverVagaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("/api/admin/estacionamentos/{estacionamentoId:guid}/vagas/{vagaId:guid}", async (
                Guid estacionamentoId,
                Guid vagaId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new RemoverVagaCommand(estacionamentoId, vagaId), cancellationToken);
                return resultado.ToNoContent();
            })
            .RequireAuthorization(Politicas.Administrador)
            .WithTags(Tags.Vagas)
            .WithSummary("Remove uma vaga que não esteja ocupada.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
