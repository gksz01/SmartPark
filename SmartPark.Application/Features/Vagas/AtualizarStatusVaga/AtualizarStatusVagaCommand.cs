using Mediator;
using SmartPark.Contracts.Vagas;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Vagas.AtualizarStatusVaga;

/// <summary>Alteração manual do status. Na fase 2 os sensores de ocupação passam a fazer isso.</summary>
public sealed record AtualizarStatusVagaCommand(Guid EstacionamentoId, Guid VagaId, string Status)
    : ICommand<Result<VagaResponse>>;
