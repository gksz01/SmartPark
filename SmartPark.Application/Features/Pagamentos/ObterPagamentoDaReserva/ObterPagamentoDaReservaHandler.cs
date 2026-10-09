using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Pagamentos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Pagamentos;
using SmartPark.Domain.Reservas;

namespace SmartPark.Application.Features.Pagamentos.ObterPagamentoDaReserva;

public sealed class ObterPagamentoDaReservaHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IQueryHandler<ObterPagamentoDaReservaQuery, Result<PagamentoResponse>>
{
    public async ValueTask<Result<PagamentoResponse>> Handle(ObterPagamentoDaReservaQuery query, CancellationToken cancellationToken)
    {
        var reservaVisivel = await db.Reservas.AnyAsync(
            r => r.Id == query.ReservaId && (r.UsuarioId == currentUser.UsuarioId || currentUser.EhAdministrador),
            cancellationToken);

        if (!reservaVisivel)
        {
            return ReservaErrors.NaoEncontrada;
        }

        var pagamento = await db.Pagamentos.AsNoTracking()
            .SingleOrDefaultAsync(p => p.ReservaId == query.ReservaId, cancellationToken);

        return pagamento is null
            ? PagamentoErrors.NaoEncontrado
            : pagamento.ParaResponse();
    }
}
