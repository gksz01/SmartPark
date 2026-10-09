using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Veiculos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Veiculos;

namespace SmartPark.Application.Features.Veiculos.AtualizarVeiculo;

public sealed class AtualizarVeiculoHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : ICommandHandler<AtualizarVeiculoCommand, Result<VeiculoResponse>>
{
    public async ValueTask<Result<VeiculoResponse>> Handle(AtualizarVeiculoCommand command, CancellationToken cancellationToken)
    {
        var veiculo = await db.Veiculos.SingleOrDefaultAsync(
            v => v.Id == command.VeiculoId && v.UsuarioId == currentUser.UsuarioId && v.Ativo,
            cancellationToken);

        if (veiculo is null)
        {
            return VeiculoErrors.NaoEncontrado;
        }

        var placa = Placa.Criar(command.Placa);
        if (placa.IsFailure)
        {
            return placa.Error;
        }

        var placaEmOutroVeiculo = await db.Veiculos.AnyAsync(
            v => v.Ativo && v.Placa == placa.Value && v.Id != veiculo.Id,
            cancellationToken);

        if (placaEmOutroVeiculo)
        {
            return VeiculoErrors.PlacaEmUso;
        }

        veiculo.Atualizar(placa.Value, command.Modelo);
        await db.SaveChangesAsync(cancellationToken);

        return veiculo.ParaResponse();
    }
}
