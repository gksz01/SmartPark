using Mediator;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Vagas.RemoverVaga;

public sealed record RemoverVagaCommand(Guid EstacionamentoId, Guid VagaId) : ICommand<Result>;
