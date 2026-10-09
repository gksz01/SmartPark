using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Veiculos.AtualizarVeiculo;
using SmartPark.Contracts.Veiculos;

namespace SmartPark.Api.Features.Veiculos;

public sealed class AtualizarVeiculoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("/api/veiculos/{veiculoId:guid}", async (Guid veiculoId, VeiculoRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new AtualizarVeiculoCommand(veiculoId, request.Placa, request.Modelo), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization(Politicas.Motorista)
            .WithTags(Tags.Veiculos)
            .WithSummary("Atualiza placa e modelo de um veículo do motorista.")
            .Produces<VeiculoResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
