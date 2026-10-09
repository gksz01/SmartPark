using Mediator;
using SmartPark.Contracts.Reservas;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Reservas.ObterReserva;

public sealed record ObterReservaQuery(Guid ReservaId) : IQuery<Result<ReservaResponse>>;
