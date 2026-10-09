using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Estacionamentos.AlterarSituacaoEstacionamento;
using SmartPark.Contracts.Estacionamentos;

namespace SmartPark.Api.Features.Estacionamentos;

public sealed class AlterarSituacaoEstacionamentoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("/api/admin/estacionamentos/{estacionamentoId:guid}/situacao", async (
                Guid estacionamentoId,
                AlterarSituacaoEstacionamentoRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new AlterarSituacaoEstacionamentoCommand(estacionamentoId, request.Ativo), cancellationToken);
                return resultado.ToNoContent();
            })
            .RequireAuthorization(Politicas.Administrador)
            .WithTags(Tags.Estacionamentos)
            .WithSummary("Ativa ou desativa o estacionamento para novas reservas.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
}
