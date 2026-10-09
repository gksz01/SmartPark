using Mediator;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Estacionamentos.AlterarSituacaoEstacionamento;

/// <summary>Ativa ou desativa o estacionamento para novas reservas. Reservas existentes são mantidas.</summary>
public sealed record AlterarSituacaoEstacionamentoCommand(Guid EstacionamentoId, bool Ativo) : ICommand<Result>;
