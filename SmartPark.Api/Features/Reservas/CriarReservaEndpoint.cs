using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Reservas.CriarReserva;
using SmartPark.Contracts.Reservas;

namespace SmartPark.Api.Features.Reservas;

public sealed class CriarReservaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/api/reservas", async (CriarReservaRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(
                    new CriarReservaCommand(request.EstacionamentoId, request.VeiculoId, request.Inicio, request.Fim),
                    cancellationToken);

                return resultado.ToCreated(r => $"/api/reservas/{r.Id}");
            })
            .RequireAuthorization(Politicas.Motorista)
            .WithTags(Tags.Reservas)
            .WithSummary("Reserva capacidade do estacionamento para um período (RF07); 409 se esgotada (RF08).")
            .Produces<ReservaResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
