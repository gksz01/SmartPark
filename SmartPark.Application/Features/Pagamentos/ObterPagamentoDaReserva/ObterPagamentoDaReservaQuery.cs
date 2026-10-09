using Mediator;
using SmartPark.Contracts.Pagamentos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Pagamentos.ObterPagamentoDaReserva;

public sealed record ObterPagamentoDaReservaQuery(Guid ReservaId) : IQuery<Result<PagamentoResponse>>;
