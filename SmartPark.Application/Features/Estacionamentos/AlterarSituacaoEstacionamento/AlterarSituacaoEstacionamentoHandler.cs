using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Estacionamentos.AlterarSituacaoEstacionamento;

public sealed class AlterarSituacaoEstacionamentoHandler(IApplicationDbContext db)
    : ICommandHandler<AlterarSituacaoEstacionamentoCommand, Result>
{
    public async ValueTask<Result> Handle(AlterarSituacaoEstacionamentoCommand command, CancellationToken cancellationToken)
    {
        var estacionamento = await db.Estacionamentos
            .SingleOrDefaultAsync(e => e.Id == command.EstacionamentoId, cancellationToken);

        if (estacionamento is null)
        {
            return EstacionamentoErrors.NaoEncontrado;
        }

        if (command.Ativo)
        {
            estacionamento.Ativar();
        }
        else
        {
            estacionamento.Desativar();
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
