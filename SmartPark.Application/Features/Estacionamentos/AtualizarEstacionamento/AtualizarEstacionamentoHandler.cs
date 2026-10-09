using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Estacionamentos.AtualizarEstacionamento;

public sealed class AtualizarEstacionamentoHandler(IApplicationDbContext db)
    : ICommandHandler<AtualizarEstacionamentoCommand, Result<EstacionamentoResponse>>
{
    public async ValueTask<Result<EstacionamentoResponse>> Handle(AtualizarEstacionamentoCommand command, CancellationToken cancellationToken)
    {
        var estacionamento = await db.Estacionamentos
            .Include(e => e.Vagas)
            .SingleOrDefaultAsync(e => e.Id == command.EstacionamentoId, cancellationToken);

        if (estacionamento is null)
        {
            return EstacionamentoErrors.NaoEncontrado;
        }

        var resultado = estacionamento.Atualizar(
            command.Nome,
            command.Endereco,
            command.Latitude,
            command.Longitude,
            command.CapacidadeTotal,
            command.CapacidadeReservavel,
            command.ValorHora);

        if (resultado.IsFailure)
        {
            return resultado.Error;
        }

        await db.SaveChangesAsync(cancellationToken);

        return estacionamento.ParaResponse(estacionamento.Vagas.Count);
    }
}
