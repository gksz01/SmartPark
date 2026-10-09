using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Vagas;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Vagas.CriarVagasEmLote;

public sealed class CriarVagasEmLoteHandler(IApplicationDbContext db)
    : ICommandHandler<CriarVagasEmLoteCommand, Result<IReadOnlyList<VagaResponse>>>
{
    public async ValueTask<Result<IReadOnlyList<VagaResponse>>> Handle(CriarVagasEmLoteCommand command, CancellationToken cancellationToken)
    {
        var estacionamento = await db.Estacionamentos
            .Include(e => e.Vagas)
            .SingleOrDefaultAsync(e => e.Id == command.EstacionamentoId, cancellationToken);

        if (estacionamento is null)
        {
            return EstacionamentoErrors.NaoEncontrado;
        }

        var novas = estacionamento.AdicionarVagas(command.Prefixo, command.Quantidade);
        if (novas.IsFailure)
        {
            return novas.Error;
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success<IReadOnlyList<VagaResponse>>([.. novas.Value.Select(v => v.ParaResponse())]);
    }
}
