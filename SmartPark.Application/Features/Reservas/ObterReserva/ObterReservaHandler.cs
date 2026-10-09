using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Reservas;
using SmartPark.Domain.Common;
using SmartPark.Domain.Reservas;

namespace SmartPark.Application.Features.Reservas.ObterReserva;

public sealed class ObterReservaHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IQueryHandler<ObterReservaQuery, Result<ReservaResponse>>
{
    public async ValueTask<Result<ReservaResponse>> Handle(ObterReservaQuery query, CancellationToken cancellationToken)
    {
        var linha = await db.Linhas().SingleOrDefaultAsync(r => r.Id == query.ReservaId, cancellationToken);

        // Reserva de outro motorista é tratada como inexistente, para não revelar que o Id existe.
        if (linha is null || linha.UsuarioId != currentUser.UsuarioId && !currentUser.EhAdministrador)
        {
            return ReservaErrors.NaoEncontrada;
        }

        return linha.ParaResponse();
    }
}
