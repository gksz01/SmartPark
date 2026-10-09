using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Veiculos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Veiculos;

namespace SmartPark.Application.Features.Veiculos.CadastrarVeiculo;

public sealed class CadastrarVeiculoHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : ICommandHandler<CadastrarVeiculoCommand, Result<VeiculoResponse>>
{
    public async ValueTask<Result<VeiculoResponse>> Handle(CadastrarVeiculoCommand command, CancellationToken cancellationToken)
    {
        var placa = Placa.Criar(command.Placa);
        if (placa.IsFailure)
        {
            return placa.Error;
        }

        if (await db.Veiculos.AnyAsync(v => v.Ativo && v.Placa == placa.Value, cancellationToken))
        {
            return VeiculoErrors.PlacaEmUso;
        }

        var veiculo = Veiculo.Criar(currentUser.UsuarioId, placa.Value, command.Modelo);
        db.Veiculos.Add(veiculo);
        await db.SaveChangesAsync(cancellationToken);

        return veiculo.ParaResponse();
    }
}
