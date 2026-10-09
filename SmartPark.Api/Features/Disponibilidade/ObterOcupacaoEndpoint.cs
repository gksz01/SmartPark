using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Disponibilidade.ObterOcupacao;
using SmartPark.Contracts.Estacionamentos;

namespace SmartPark.Api.Features.Disponibilidade;

public sealed class ObterOcupacaoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/estacionamentos/{estacionamentoId:guid}/ocupacao", async (Guid estacionamentoId, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ObterOcupacaoQuery(estacionamentoId), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization()
            .WithTags(Tags.Disponibilidade)
            .WithSummary("Ocupação física atual das vagas (RF06).")
            .Produces<OcupacaoResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
}
