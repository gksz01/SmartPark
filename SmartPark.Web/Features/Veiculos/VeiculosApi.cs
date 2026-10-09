using SmartPark.Contracts.Veiculos;
using SmartPark.Web.Autenticacao;
using SmartPark.Web.Comum;

namespace SmartPark.Web.Features.Veiculos;

public sealed class VeiculosApi(HttpClient http, SessaoStore sessao) : ApiClientBase(http, sessao)
{
    public Task<ApiResult<List<VeiculoResponse>>> ListarAsync() =>
        GetAsync<List<VeiculoResponse>>("/api/veiculos");

    public Task<ApiResult<VeiculoResponse>> CadastrarAsync(VeiculoRequest request) =>
        PostAsync<VeiculoResponse>("/api/veiculos", request);

    public Task<ApiResult<VeiculoResponse>> AtualizarAsync(Guid veiculoId, VeiculoRequest request) =>
        PutAsync<VeiculoResponse>($"/api/veiculos/{veiculoId}", request);

    public Task<ApiResult> RemoverAsync(Guid veiculoId) =>
        EnviarSemRetornoAsync(HttpMethod.Delete, $"/api/veiculos/{veiculoId}");
}
