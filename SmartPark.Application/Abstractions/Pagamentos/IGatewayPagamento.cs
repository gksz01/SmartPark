using SmartPark.Domain.Pagamentos;

namespace SmartPark.Application.Abstractions.Pagamentos;

/// <summary>
/// Porta para o gateway de pagamento externo. A implementação não lança exceção quando o gateway está
/// fora do ar: devolve <see cref="SituacaoCobranca.Indisponivel"/>, e quem chama decide o que fazer.
/// </summary>
public interface IGatewayPagamento
{
    Task<ResultadoCobranca> CobrarAsync(SolicitacaoCobranca solicitacao, CancellationToken cancellationToken);

    Task<ResultadoEstorno> EstornarAsync(string transacaoId, decimal valor, CancellationToken cancellationToken);
}

public sealed record SolicitacaoCobranca(Guid PagamentoId, decimal Valor, MetodoPagamento Metodo, string? NumeroCartao);

public enum SituacaoCobranca
{
    Aprovada,
    Recusada,
    Indisponivel
}

public sealed record ResultadoCobranca(SituacaoCobranca Situacao, string? TransacaoId, string? MotivoRecusa);

public sealed record ResultadoEstorno(bool Sucesso);
