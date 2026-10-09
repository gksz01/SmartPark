using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Application.Abstractions.Pagamentos;
using SmartPark.Application.Common;
using SmartPark.Contracts.Pagamentos;
using SmartPark.Domain.Common;
using SmartPark.Domain.Pagamentos;
using SmartPark.Domain.Reservas;

namespace SmartPark.Application.Features.Pagamentos.PagarReserva;

public sealed class PagarReservaHandler(
    IApplicationDbContext db,
    ICurrentUser currentUser,
    IGatewayPagamento gatewayPagamento,
    TimeProvider timeProvider)
    : ICommandHandler<PagarReservaCommand, Result<PagamentoResponse>>
{
    public async ValueTask<Result<PagamentoResponse>> Handle(PagarReservaCommand command, CancellationToken cancellationToken)
    {
        var reserva = await db.Reservas.AsNoTracking().SingleOrDefaultAsync(
            r => r.Id == command.ReservaId && r.UsuarioId == currentUser.UsuarioId,
            cancellationToken);

        if (reserva is null)
        {
            return ReservaErrors.NaoEncontrada;
        }

        var metodo = EnumTexto.Converter<MetodoPagamento>(command.Metodo);
        var pagamento = await db.Pagamentos.SingleOrDefaultAsync(p => p.ReservaId == reserva.Id, cancellationToken);

        if (pagamento is null)
        {
            var novo = Pagamento.Iniciar(reserva, metodo);
            if (novo.IsFailure)
            {
                return novo.Error;
            }

            pagamento = novo.Value;
            db.Pagamentos.Add(pagamento);
        }
        else
        {
            var tentativa = pagamento.NovaTentativa(reserva, metodo);
            if (tentativa.IsFailure)
            {
                return tentativa.Error;
            }
        }

        // Registra a tentativa como Pendente antes de falar com o gateway: se ele cair no meio, o estado
        // persistido continua coerente e a saída nunca é liberada por um pagamento não confirmado.
        await db.SaveChangesAsync(cancellationToken);

        var cobranca = await gatewayPagamento.CobrarAsync(
            new SolicitacaoCobranca(pagamento.Id, pagamento.Valor, metodo, command.NumeroCartao),
            cancellationToken);

        switch (cobranca.Situacao)
        {
            case SituacaoCobranca.Aprovada:
                pagamento.Aprovar(cobranca.TransacaoId!, timeProvider.GetUtcNow());
                break;
            case SituacaoCobranca.Recusada:
                pagamento.Recusar(cobranca.MotivoRecusa ?? "Pagamento não autorizado.", timeProvider.GetUtcNow());
                break;
            default:
                return PagamentoErrors.GatewayIndisponivel;
        }

        await db.SaveChangesAsync(cancellationToken);
        return pagamento.ParaResponse();
    }
}
