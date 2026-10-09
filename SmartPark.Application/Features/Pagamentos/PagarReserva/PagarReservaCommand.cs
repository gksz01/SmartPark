using Mediator;
using SmartPark.Contracts.Pagamentos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Pagamentos.PagarReserva;

/// <summary>
/// Paga o valor estimado da reserva pelo gateway externo (RF10). Uma recusa é um resultado válido do
/// pagamento — volta com status Recusado e o motivo — e não um erro da requisição.
/// </summary>
public sealed record PagarReservaCommand(Guid ReservaId, string Metodo, string? NumeroCartao)
    : ICommand<Result<PagamentoResponse>>;
