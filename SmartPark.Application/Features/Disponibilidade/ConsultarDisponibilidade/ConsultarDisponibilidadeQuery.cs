using Mediator;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Disponibilidade.ConsultarDisponibilidade;

public sealed record ConsultarDisponibilidadeQuery(Guid EstacionamentoId, DateTimeOffset Inicio, DateTimeOffset Fim)
    : IQuery<Result<DisponibilidadeResponse>>;
