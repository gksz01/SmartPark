using SmartPark.Contracts.Pagamentos;
using SmartPark.Contracts.Reservas;
using SmartPark.Web.Autenticacao;
using SmartPark.Web.Comum;

namespace SmartPark.Web.Features.Reservas;

public sealed class ReservasApi(HttpClient http, SessaoStore sessao) : ApiClientBase(http, sessao)
{
    public Task<ApiResult<ReservaResponse>> CriarAsync(CriarReservaRequest request) =>
        PostAsync<ReservaResponse>("/api/reservas", request);

    public Task<ApiResult<List<ReservaResponse>>> ListarAsync() =>
        GetAsync<List<ReservaResponse>>("/api/reservas");

    public Task<ApiResult> CancelarAsync(Guid reservaId) =>
        EnviarSemRetornoAsync(HttpMethod.Post, $"/api/reservas/{reservaId}/cancelamento");

    public Task<ApiResult<PagamentoResponse>> PagarAsync(Guid reservaId, PagarReservaRequest request) =>
        PostAsync<PagamentoResponse>($"/api/reservas/{reservaId}/pagamento", request);
}
