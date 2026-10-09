using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartPark.Application.Abstractions.Pagamentos;
using SmartPark.Domain.Pagamentos;

namespace SmartPark.Infrastructure.Pagamentos;

/// <summary>
/// Gateway de pagamento simulado da fase 1. Aprova tudo, exceto cartões terminados em 0000, e pode
/// simular indisponibilidade por configuração. Será trocado pelo adaptador do gateway real sem mudar
/// nenhuma fatia, porque elas dependem só de <see cref="IGatewayPagamento"/>.
/// </summary>
internal sealed partial class GatewayPagamentoSimulado(
    IOptionsMonitor<GatewayPagamentoOptions> options,
    ILogger<GatewayPagamentoSimulado> logger) : IGatewayPagamento
{
    public async Task<ResultadoCobranca> CobrarAsync(SolicitacaoCobranca solicitacao, CancellationToken cancellationToken)
    {
        await Task.Delay(options.CurrentValue.LatenciaMs, cancellationToken);

        if (options.CurrentValue.SimularIndisponibilidade)
        {
            LogIndisponivel(logger, solicitacao.PagamentoId);
            return new ResultadoCobranca(SituacaoCobranca.Indisponivel, null, null);
        }

        if (solicitacao.Metodo == MetodoPagamento.Cartao && solicitacao.NumeroCartao?.EndsWith("0000", StringComparison.Ordinal) == true)
        {
            LogRecusado(logger, solicitacao.PagamentoId);
            return new ResultadoCobranca(SituacaoCobranca.Recusada, null, "Cartão recusado pelo emissor.");
        }

        var transacao = $"SIM-{Guid.NewGuid():N}";
        LogAprovado(logger, solicitacao.PagamentoId, solicitacao.Valor, transacao);
        return new ResultadoCobranca(SituacaoCobranca.Aprovada, transacao, null);
    }

    public async Task<ResultadoEstorno> EstornarAsync(string transacaoId, decimal valor, CancellationToken cancellationToken)
    {
        await Task.Delay(options.CurrentValue.LatenciaMs, cancellationToken);
        return new ResultadoEstorno(!options.CurrentValue.SimularIndisponibilidade);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Pagamento {PagamentoId} de {Valor} aprovado ({Transacao})")]
    private static partial void LogAprovado(ILogger logger, Guid pagamentoId, decimal valor, string transacao);

    [LoggerMessage(Level = LogLevel.Information, Message = "Pagamento {PagamentoId} recusado")]
    private static partial void LogRecusado(ILogger logger, Guid pagamentoId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Gateway simulado indisponível para o pagamento {PagamentoId}")]
    private static partial void LogIndisponivel(ILogger logger, Guid pagamentoId);
}
