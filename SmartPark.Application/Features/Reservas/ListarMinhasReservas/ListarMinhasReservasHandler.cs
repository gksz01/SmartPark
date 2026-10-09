using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Reservas;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Reservas.ListarMinhasReservas;

public sealed class ListarMinhasReservasHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IQueryHandler<ListarMinhasReservasQuery, Result<IReadOnlyList<ReservaResponse>>>
{
    public async ValueTask<Result<IReadOnlyList<ReservaResponse>>> Handle(ListarMinhasReservasQuery query, CancellationToken cancellationToken)
    {
        var linhas = await db.Linhas()
            .Where(r => r.UsuarioId == currentUser.UsuarioId)
            .OrderByDescending(r => r.Inicio)
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<ReservaResponse>>([.. linhas.Select(l => l.ParaResponse())]);
    }
}
