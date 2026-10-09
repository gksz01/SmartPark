using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Reservas.ObterReserva;
using SmartPark.Contracts.Reservas;

namespace SmartPark.Api.Features.Reservas;

public sealed class ObterReservaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/reservas/{reservaId:guid}", async (Guid reservaId, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ObterReservaQuery(reservaId), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization()
            .WithTags(Tags.Reservas)
            .WithSummary("Detalhes de uma reserva do motorista (administradores veem todas).")
            .Produces<ReservaResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
}
