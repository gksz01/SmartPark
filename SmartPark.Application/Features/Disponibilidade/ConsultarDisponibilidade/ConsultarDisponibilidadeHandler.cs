using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Application.Common;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;
using SmartPark.Domain.Reservas;

namespace SmartPark.Application.Features.Disponibilidade.ConsultarDisponibilidade;

public sealed class ConsultarDisponibilidadeHandler(IApplicationDbContext db, TimeProvider timeProvider)
    : IQueryHandler<ConsultarDisponibilidadeQuery, Result<DisponibilidadeResponse>>
{
    public async ValueTask<Result<DisponibilidadeResponse>> Handle(ConsultarDisponibilidadeQuery query, CancellationToken cancellationToken)
    {
        var periodo = new Periodo(query.Inicio.ToUniversalTime(), query.Fim.ToUniversalTime());

        var periodoValido = Reserva.ValidarPeriodo(periodo, timeProvider.GetUtcNow());
        if (periodoValido.IsFailure)
        {
            return periodoValido.Error;
        }

        var estacionamento = await db.Estacionamentos.AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == query.EstacionamentoId && e.Ativo, cancellationToken);

        if (estacionamento is null)
        {
            return EstacionamentoErrors.NaoEncontrado;
        }

        var reservasAtivas = await db.Reservas.AsNoTracking()
            .Where(r => r.EstacionamentoId == estacionamento.Id)
            .QueOcupamCapacidade()
            .QueSobrepoem(periodo)
            .Select(r => new Periodo(r.Inicio, r.Fim))
            .ToListAsync(cancellationToken);

        var disponivel = estacionamento.CapacidadeDisponivel(periodo, reservasAtivas);

        return new DisponibilidadeResponse(
            estacionamento.Id,
            periodo.Inicio,
            periodo.Fim,
            estacionamento.CapacidadeReservavel,
            estacionamento.CapacidadeReservavel - disponivel,
            disponivel,
            estacionamento.CalcularValor(periodo));
    }
}
