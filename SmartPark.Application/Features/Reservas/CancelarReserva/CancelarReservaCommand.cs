using Mediator;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Reservas.CancelarReserva;

/// <summary>Cancela a reserva e, se ela já estiver paga, estorna o pagamento (RF09).</summary>
public sealed record CancelarReservaCommand(Guid ReservaId) : ICommand<Result>;
