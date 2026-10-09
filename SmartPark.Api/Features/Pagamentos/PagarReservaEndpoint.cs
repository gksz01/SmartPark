using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Pagamentos.PagarReserva;
using SmartPark.Contracts.Pagamentos;

namespace SmartPark.Api.Features.Pagamentos;

public sealed class PagarReservaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/api/reservas/{reservaId:guid}/pagamento", async (
                Guid reservaId,
                PagarReservaRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new PagarReservaCommand(reservaId, request.Metodo, request.NumeroCartao), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization(Politicas.Motorista)
            .WithTags(Tags.Pagamentos)
            .WithSummary("Paga a reserva pelo gateway (RF10). Recusa volta como status Recusado.")
            .Produces<PagamentoResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);
}
