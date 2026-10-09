using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Application.Common;
using SmartPark.Domain.Common;
using SmartPark.Domain.Veiculos;

namespace SmartPark.Application.Features.Veiculos.RemoverVeiculo;

public sealed class RemoverVeiculoHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : ICommandHandler<RemoverVeiculoCommand, Result>
{
    public async ValueTask<Result> Handle(RemoverVeiculoCommand command, CancellationToken cancellationToken)
    {
        var veiculo = await db.Veiculos.SingleOrDefaultAsync(
            v => v.Id == command.VeiculoId && v.UsuarioId == currentUser.UsuarioId && v.Ativo,
            cancellationToken);

        if (veiculo is null)
        {
            return VeiculoErrors.NaoEncontrado;
        }

        var possuiReservaAtiva = await db.Reservas
            .QueOcupamCapacidade()
            .AnyAsync(r => r.VeiculoId == veiculo.Id, cancellationToken);

        if (possuiReservaAtiva)
        {
            return VeiculoErrors.PossuiReservaAtiva;
        }

        veiculo.Remover();
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
