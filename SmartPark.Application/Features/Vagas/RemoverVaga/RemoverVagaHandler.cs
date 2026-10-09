using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Vagas.RemoverVaga;

public sealed class RemoverVagaHandler(IApplicationDbContext db)
    : ICommandHandler<RemoverVagaCommand, Result>
{
    public async ValueTask<Result> Handle(RemoverVagaCommand command, CancellationToken cancellationToken)
    {
        var estacionamento = await db.Estacionamentos
            .Include(e => e.Vagas)
            .SingleOrDefaultAsync(e => e.Id == command.EstacionamentoId, cancellationToken);

        if (estacionamento is null)
        {
            return EstacionamentoErrors.NaoEncontrado;
        }

        var resultado = estacionamento.RemoverVaga(command.VagaId);
        if (resultado.IsFailure)
        {
            return resultado;
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
