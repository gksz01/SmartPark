using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Application.Features.Autenticacao.ObterUsuarioAtual;

public sealed class ObterUsuarioAtualHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IQueryHandler<ObterUsuarioAtualQuery, Result<UsuarioResponse>>
{
    public async ValueTask<Result<UsuarioResponse>> Handle(ObterUsuarioAtualQuery query, CancellationToken cancellationToken)
    {
        var usuario = await db.Usuarios.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == currentUser.UsuarioId, cancellationToken);

        return usuario is null
            ? UsuarioErrors.NaoEncontrado
            : usuario.ParaResponse();
    }
}
