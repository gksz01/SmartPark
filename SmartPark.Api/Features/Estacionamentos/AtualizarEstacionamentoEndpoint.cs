using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Estacionamentos.AtualizarEstacionamento;
using SmartPark.Contracts.Estacionamentos;

namespace SmartPark.Api.Features.Estacionamentos;

public sealed class AtualizarEstacionamentoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("/api/admin/estacionamentos/{estacionamentoId:guid}", async (
                Guid estacionamentoId,
                EstacionamentoRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(
                    new AtualizarEstacionamentoCommand(
                        estacionamentoId,
                        request.Nome,
                        request.Endereco,
                        request.Latitude,
                        request.Longitude,
                        request.CapacidadeTotal,
                        request.CapacidadeReservavel,
                        request.ValorHora),
                    cancellationToken);

                return resultado.ToOk();
            })
            .RequireAuthorization(Politicas.Administrador)
            .WithTags(Tags.Estacionamentos)
            .WithSummary("Atualiza os dados e a capacidade de um estacionamento (RF24).")
            .Produces<EstacionamentoResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
