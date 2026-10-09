using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Reservas.ListarMinhasReservas;
using SmartPark.Contracts.Reservas;

namespace SmartPark.Api.Features.Reservas;

public sealed class ListarMinhasReservasEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/reservas", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ListarMinhasReservasQuery(), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization(Politicas.Motorista)
            .WithTags(Tags.Reservas)
            .WithSummary("Reservas do motorista, das mais recentes para as mais antigas (RF09).")
            .Produces<IReadOnlyList<ReservaResponse>>();
}
