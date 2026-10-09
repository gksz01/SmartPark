using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Veiculos.CadastrarVeiculo;
using SmartPark.Contracts.Veiculos;

namespace SmartPark.Api.Features.Veiculos;

public sealed class CadastrarVeiculoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/api/veiculos", async (VeiculoRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new CadastrarVeiculoCommand(request.Placa, request.Modelo), cancellationToken);
                return resultado.ToCreated(v => $"/api/veiculos/{v.Id}");
            })
            .RequireAuthorization(Politicas.Motorista)
            .WithTags(Tags.Veiculos)
            .WithSummary("Cadastra um veículo do motorista (RF03).")
            .Produces<VeiculoResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
