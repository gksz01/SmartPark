using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Vagas;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Vagas.ListarVagas;

public sealed class ListarVagasHandler(IApplicationDbContext db)
    : IQueryHandler<ListarVagasQuery, Result<IReadOnlyList<VagaResponse>>>
{
    public async ValueTask<Result<IReadOnlyList<VagaResponse>>> Handle(ListarVagasQuery query, CancellationToken cancellationToken)
    {
        if (!await db.Estacionamentos.AnyAsync(e => e.Id == query.EstacionamentoId, cancellationToken))
        {
            return EstacionamentoErrors.NaoEncontrado;
        }

        var vagas = await db.Vagas.AsNoTracking()
            .Where(v => v.EstacionamentoId == query.EstacionamentoId)
            .OrderBy(v => v.Identificador)
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<VagaResponse>>([.. vagas.Select(v => v.ParaResponse())]);
    }
}
