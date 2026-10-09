using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Estacionamentos.ObterEstacionamento;

public sealed class ObterEstacionamentoHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IQueryHandler<ObterEstacionamentoQuery, Result<EstacionamentoResponse>>
{
    public async ValueTask<Result<EstacionamentoResponse>> Handle(ObterEstacionamentoQuery query, CancellationToken cancellationToken)
    {
        var linha = await db.Estacionamentos.AsNoTracking()
            .Where(e => e.Id == query.EstacionamentoId)
            .Select(e => new { Estacionamento = e, TotalVagas = e.Vagas.Count() })
            .SingleOrDefaultAsync(cancellationToken);

        // Inativo é invisível para quem não administra.
        if (linha is null || !linha.Estacionamento.Ativo && !currentUser.EhAdministrador)
        {
            return EstacionamentoErrors.NaoEncontrado;
        }

        return linha.Estacionamento.ParaResponse(linha.TotalVagas);
    }
}
