using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Application.Common;
using SmartPark.Contracts.Reservas;
using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;
using SmartPark.Domain.Reservas;

namespace SmartPark.Application.Features.Reservas.CriarReserva;

/// <summary>
/// Confirma uma reserva de capacidade sem overbooking (RF07/RF08). A checagem de capacidade e a gravação
/// são protegidas pelo token de concorrência do estacionamento: se outra reserva for confirmada entre a
/// leitura e a gravação, a gravação falha e a capacidade é recalculada com o novo estado.
/// </summary>
public sealed class CriarReservaHandler(IApplicationDbContext db, ICurrentUser currentUser, TimeProvider timeProvider)
    : ICommandHandler<CriarReservaCommand, Result<ReservaResponse>>
{
    internal const int MaximoTentativas = 3;

    private static readonly Error ConcorrenciaExcessiva = Error.Conflict(
        "Reserva.ConcorrenciaExcessiva",
        "Muitas reservas simultâneas para este estacionamento. Tente novamente.");

    public async ValueTask<Result<ReservaResponse>> Handle(CriarReservaCommand command, CancellationToken cancellationToken)
    {
        var periodo = new Periodo(command.Inicio.ToUniversalTime(), command.Fim.ToUniversalTime());

        for (var tentativa = 1; tentativa <= MaximoTentativas; tentativa++)
        {
            var estacionamento = await db.Estacionamentos
                .SingleOrDefaultAsync(e => e.Id == command.EstacionamentoId, cancellationToken);

            if (estacionamento is null)
            {
                return EstacionamentoErrors.NaoEncontrado;
            }

            var veiculo = await db.Veiculos.AsNoTracking()
                .SingleOrDefaultAsync(v => v.Id == command.VeiculoId, cancellationToken);

            if (veiculo is null)
            {
                return ReservaErrors.VeiculoInvalido;
            }

            var reservasAtivas = await db.Reservas.AsNoTracking()
                .Where(r => r.EstacionamentoId == estacionamento.Id)
                .QueOcupamCapacidade()
                .QueSobrepoem(periodo)
                .Select(r => new Periodo(r.Inicio, r.Fim))
                .ToListAsync(cancellationToken);

            var reserva = Reserva.Criar(
                currentUser.UsuarioId,
                veiculo,
                estacionamento,
                periodo,
                reservasAtivas,
                timeProvider.GetUtcNow());

            if (reserva.IsFailure)
            {
                return reserva.Error;
            }

            db.Reservas.Add(reserva.Value);

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Outra reserva mudou o estacionamento no meio do caminho: descarta tudo e revalida.
                db.ChangeTracker.Clear();
                continue;
            }

            return new ReservaLinha
            {
                Id = reserva.Value.Id,
                UsuarioId = reserva.Value.UsuarioId,
                EstacionamentoId = estacionamento.Id,
                EstacionamentoNome = estacionamento.Nome,
                VeiculoId = veiculo.Id,
                Placa = veiculo.Placa,
                Inicio = reserva.Value.Inicio,
                Fim = reserva.Value.Fim,
                ValorEstimado = reserva.Value.ValorEstimado,
                Status = reserva.Value.Status,
                StatusPagamento = null,
                CriadoEm = reserva.Value.CriadoEm
            }.ParaResponse();
        }

        return ConcorrenciaExcessiva;
    }
}
