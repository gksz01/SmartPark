using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Vagas.AtualizarStatusVaga;
using SmartPark.Contracts.Vagas;

namespace SmartPark.Api.Features.Vagas;

public sealed class AtualizarStatusVagaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("/api/admin/estacionamentos/{estacionamentoId:guid}/vagas/{vagaId:guid}/status", async (
                Guid estacionamentoId,
                Guid vagaId,
                AtualizarStatusVagaRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new AtualizarStatusVagaCommand(estacionamentoId, vagaId, request.Status), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization(Politicas.Administrador)
            .WithTags(Tags.Vagas)
            .WithSummary("Altera manualmente o status de uma vaga (sensores na fase 2).")
            .Produces<VagaResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
}
