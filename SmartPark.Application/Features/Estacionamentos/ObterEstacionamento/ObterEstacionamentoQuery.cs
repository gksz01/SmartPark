using Mediator;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Estacionamentos.ObterEstacionamento;

public sealed record ObterEstacionamentoQuery(Guid EstacionamentoId) : IQuery<Result<EstacionamentoResponse>>;
