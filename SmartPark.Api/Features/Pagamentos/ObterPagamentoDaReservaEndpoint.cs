using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Pagamentos.ObterPagamentoDaReserva;
using SmartPark.Contracts.Pagamentos;

namespace SmartPark.Api.Features.Pagamentos;

public sealed class ObterPagamentoDaReservaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/reservas/{reservaId:guid}/pagamento", async (Guid reservaId, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ObterPagamentoDaReservaQuery(reservaId), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization()
            .WithTags(Tags.Pagamentos)
            .WithSummary("Situação do pagamento de uma reserva (RF10).")
            .Produces<PagamentoResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
}
