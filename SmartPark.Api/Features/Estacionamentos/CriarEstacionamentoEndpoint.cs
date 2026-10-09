using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Estacionamentos.CriarEstacionamento;
using SmartPark.Contracts.Estacionamentos;

namespace SmartPark.Api.Features.Estacionamentos;

public sealed class CriarEstacionamentoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/api/admin/estacionamentos", async (EstacionamentoRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(
                    new CriarEstacionamentoCommand(
                        request.Nome,
                        request.Endereco,
                        request.Latitude,
                        request.Longitude,
                        request.CapacidadeTotal,
                        request.CapacidadeReservavel,
                        request.ValorHora),
                    cancellationToken);

                return resultado.ToCreated(e => $"/api/estacionamentos/{e.Id}");
            })
            .RequireAuthorization(Politicas.Administrador)
            .WithTags(Tags.Estacionamentos)
            .WithSummary("Cadastra um estacionamento (RF24).")
            .Produces<EstacionamentoResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
}
