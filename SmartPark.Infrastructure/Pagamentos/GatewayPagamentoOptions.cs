namespace SmartPark.Infrastructure.Pagamentos;

public sealed class GatewayPagamentoOptions
{
    public const string Secao = "GatewayPagamento";

    /// <summary>Liga o cenário "gateway fora do ar" para testar a degradação (RE01).</summary>
    public bool SimularIndisponibilidade { get; set; }

    public int LatenciaMs { get; set; } = 400;
}
