using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Application.Common;
using SmartPark.Contracts.Vagas;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Vagas.AtualizarStatusVaga;

public sealed class AtualizarStatusVagaHandler(IApplicationDbContext db)
    : ICommandHandler<AtualizarStatusVagaCommand, Result<VagaResponse>>
{
    public async ValueTask<Result<VagaResponse>> Handle(AtualizarStatusVagaCommand command, CancellationToken cancellationToken)
    {
        var estacionamento = await db.Estacionamentos
            .Include(e => e.Vagas)
            .SingleOrDefaultAsync(e => e.Id == command.EstacionamentoId, cancellationToken);

        if (estacionamento is null)
        {
            return EstacionamentoErrors.NaoEncontrado;
        }

        var resultado = estacionamento.AlterarStatusVaga(command.VagaId, EnumTexto.Converter<StatusVaga>(command.Status));
        if (resultado.IsFailure)
        {
            return resultado.Error;
        }

        await db.SaveChangesAsync(cancellationToken);

        return estacionamento.Vagas.Single(v => v.Id == command.VagaId).ParaResponse();
    }
}
