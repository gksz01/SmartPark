using Mediator;
using SmartPark.Contracts.Reservas;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Reservas.ListarMinhasReservas;

public sealed record ListarMinhasReservasQuery : IQuery<Result<IReadOnlyList<ReservaResponse>>>;
