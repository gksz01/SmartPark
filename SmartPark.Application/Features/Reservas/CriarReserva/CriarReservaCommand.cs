using Mediator;
using SmartPark.Contracts.Reservas;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Reservas.CriarReserva;

public sealed record CriarReservaCommand(Guid EstacionamentoId, Guid VeiculoId, DateTimeOffset Inicio, DateTimeOffset Fim)
    : ICommand<Result<ReservaResponse>>;
