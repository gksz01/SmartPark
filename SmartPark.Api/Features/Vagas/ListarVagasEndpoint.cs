using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Vagas.ListarVagas;
using SmartPark.Contracts.Vagas;

namespace SmartPark.Api.Features.Vagas;

public sealed class ListarVagasEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/admin/estacionamentos/{estacionamentoId:guid}/vagas", async (Guid estacionamentoId, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ListarVagasQuery(estacionamentoId), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization(Politicas.Administrador)
            .WithTags(Tags.Vagas)
            .WithSummary("Lista as vagas de um estacionamento com o status atual.")
            .Produces<IReadOnlyList<VagaResponse>>()
            .ProducesProblem(StatusCodes.Status404NotFound);
}
