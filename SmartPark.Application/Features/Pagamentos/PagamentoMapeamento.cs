using SmartPark.Contracts.Pagamentos;
using SmartPark.Domain.Pagamentos;

namespace SmartPark.Application.Features.Pagamentos;

internal static class PagamentoMapeamento
{
    public static PagamentoResponse ParaResponse(this Pagamento pagamento) =>
        new(
            pagamento.Id,
            pagamento.ReservaId,
            pagamento.Valor,
            pagamento.Metodo.ToString(),
            pagamento.Status.ToString(),
            pagamento.ProcessadoEm,
            pagamento.TransacaoExternaId,
            pagamento.MotivoRecusa);
}
