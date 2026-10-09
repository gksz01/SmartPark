using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Application.Common;
using SmartPark.Application.Features.Autenticacao;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Application.Features.Usuarios.ListarUsuarios;

public sealed class ListarUsuariosHandler(IApplicationDbContext db)
    : IQueryHandler<ListarUsuariosQuery, Result<IReadOnlyList<UsuarioResponse>>>
{
    public async ValueTask<Result<IReadOnlyList<UsuarioResponse>>> Handle(ListarUsuariosQuery query, CancellationToken cancellationToken)
    {
        var usuarios = db.Usuarios.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Perfil))
        {
            var perfil = EnumTexto.Converter<Perfil>(query.Perfil);
            usuarios = usuarios.Where(u => u.Perfil == perfil);
        }

        var lista = await usuarios.OrderBy(u => u.Nome).ToListAsync(cancellationToken);

        // Conversões implícitas não se aplicam a interfaces, daí o Success explícito.
        return Result.Success<IReadOnlyList<UsuarioResponse>>([.. lista.Select(u => u.ParaResponse())]);
    }
}
