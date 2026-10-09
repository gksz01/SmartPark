using Mediator;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Estacionamentos.CriarEstacionamento;

public sealed class CriarEstacionamentoHandler(IApplicationDbContext db)
    : ICommandHandler<CriarEstacionamentoCommand, Result<EstacionamentoResponse>>
{
    public async ValueTask<Result<EstacionamentoResponse>> Handle(CriarEstacionamentoCommand command, CancellationToken cancellationToken)
    {
        var estacionamento = Estacionamento.Criar(
            command.Nome,
            command.Endereco,
            command.Latitude,
            command.Longitude,
            command.CapacidadeTotal,
            command.CapacidadeReservavel,
            command.ValorHora);

        if (estacionamento.IsFailure)
        {
            return estacionamento.Error;
        }

        db.Estacionamentos.Add(estacionamento.Value);
        await db.SaveChangesAsync(cancellationToken);

        return estacionamento.Value.ParaResponse(totalVagas: 0);
    }
}
