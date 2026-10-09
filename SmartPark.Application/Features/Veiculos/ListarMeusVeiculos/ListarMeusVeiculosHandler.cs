using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Veiculos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Veiculos.ListarMeusVeiculos;

public sealed class ListarMeusVeiculosHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IQueryHandler<ListarMeusVeiculosQuery, Result<IReadOnlyList<VeiculoResponse>>>
{
    public async ValueTask<Result<IReadOnlyList<VeiculoResponse>>> Handle(ListarMeusVeiculosQuery query, CancellationToken cancellationToken)
    {
        var veiculos = await db.Veiculos.AsNoTracking()
            .Where(v => v.UsuarioId == currentUser.UsuarioId && v.Ativo)
            .OrderBy(v => v.Modelo)
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<VeiculoResponse>>([.. veiculos.Select(v => v.ParaResponse())]);
    }
}
