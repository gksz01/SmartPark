using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Vagas.CriarVagasEmLote;
using SmartPark.Contracts.Vagas;

namespace SmartPark.Api.Features.Vagas;

public sealed class CriarVagasEmLoteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/api/admin/estacionamentos/{estacionamentoId:guid}/vagas", async (
                Guid estacionamentoId,
                CriarVagasEmLoteRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(
                    new CriarVagasEmLoteCommand(estacionamentoId, request.Prefixo, request.Quantidade),
                    cancellationToken);

                return resultado.ToCreated(_ => $"/api/admin/estacionamentos/{estacionamentoId}/vagas");
            })
            .RequireAuthorization(Politicas.Administrador)
            .WithTags(Tags.Vagas)
            .WithSummary("Cria vagas em lote, numeradas a partir de um prefixo (RF24).")
            .Produces<IReadOnlyList<VagaResponse>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
