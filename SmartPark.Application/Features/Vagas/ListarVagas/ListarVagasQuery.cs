using Mediator;
using SmartPark.Contracts.Vagas;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Vagas.ListarVagas;

public sealed record ListarVagasQuery(Guid EstacionamentoId) : IQuery<Result<IReadOnlyList<VagaResponse>>>;
