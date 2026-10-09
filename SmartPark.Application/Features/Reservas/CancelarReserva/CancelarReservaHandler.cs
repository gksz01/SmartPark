using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Application.Abstractions.Pagamentos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Pagamentos;
using SmartPark.Domain.Reservas;

namespace SmartPark.Application.Features.Reservas.CancelarReserva;

public sealed class CancelarReservaHandler(
    IApplicationDbContext db,
    ICurrentUser currentUser,
    IGatewayPagamento gatewayPagamento,
    TimeProvider timeProvider)
    : ICommandHandler<CancelarReservaCommand, Result>
{
    public async ValueTask<Result> Handle(CancelarReservaCommand command, CancellationToken cancellationToken)
    {
        var reserva = await db.Reservas.SingleOrDefaultAsync(
            r => r.Id == command.ReservaId && r.UsuarioId == currentUser.UsuarioId,
            cancellationToken);

        if (reserva is null)
        {
            return ReservaErrors.NaoEncontrada;
        }

        var agora = timeProvider.GetUtcNow();
        var cancelamento = reserva.Cancelar(agora);
        if (cancelamento.IsFailure)
        {
            return cancelamento;
        }

        var pagamento = await db.Pagamentos.SingleOrDefaultAsync(p => p.ReservaId == reserva.Id, cancellationToken);

        if (pagamento is { Status: StatusPagamento.Aprovado })
        {
            // O estorno vem antes da gravação: se o gateway estiver fora, nada muda e o motorista tenta de novo.
            var estorno = await gatewayPagamento.EstornarAsync(pagamento.TransacaoExternaId!, pagamento.Valor, cancellationToken);
            if (!estorno.Sucesso)
            {
                return PagamentoErrors.GatewayIndisponivel;
            }

            pagamento.Estornar(agora);
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
