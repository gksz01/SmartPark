using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Disponibilidade.ObterOcupacao;

public sealed class ObterOcupacaoHandler(IApplicationDbContext db)
    : IQueryHandler<ObterOcupacaoQuery, Result<OcupacaoResponse>>
{
    public async ValueTask<Result<OcupacaoResponse>> Handle(ObterOcupacaoQuery query, CancellationToken cancellationToken)
    {
        if (!await db.Estacionamentos.AnyAsync(e => e.Id == query.EstacionamentoId, cancellationToken))
        {
            return EstacionamentoErrors.NaoEncontrado;
        }

        var porStatus = await db.Vagas.AsNoTracking()
            .Where(v => v.EstacionamentoId == query.EstacionamentoId)
            .GroupBy(v => v.Status)
            .Select(g => new { Status = g.Key, Quantidade = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Quantidade, cancellationToken);

        var livres = porStatus.GetValueOrDefault(StatusVaga.Livre);
        var ocupadas = porStatus.GetValueOrDefault(StatusVaga.Ocupada);
        var indisponiveis = porStatus.GetValueOrDefault(StatusVaga.Indisponivel);

        return new OcupacaoResponse(query.EstacionamentoId, livres + ocupadas + indisponiveis, livres, ocupadas, indisponiveis);
    }
}
