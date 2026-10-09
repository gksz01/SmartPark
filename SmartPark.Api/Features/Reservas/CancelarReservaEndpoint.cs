using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Reservas.CancelarReserva;

namespace SmartPark.Api.Features.Reservas;

public sealed class CancelarReservaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/api/reservas/{reservaId:guid}/cancelamento", async (Guid reservaId, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new CancelarReservaCommand(reservaId), cancellationToken);
                return resultado.ToNoContent();
            })
            .RequireAuthorization(Politicas.Motorista)
            .WithTags(Tags.Reservas)
            .WithSummary("Cancela uma reserva que ainda não começou, estornando o pagamento se houver (RF09).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);
}
