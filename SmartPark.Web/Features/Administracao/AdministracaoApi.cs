using SmartPark.Contracts.Autenticacao;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Contracts.Usuarios;
using SmartPark.Contracts.Vagas;
using SmartPark.Web.Autenticacao;
using SmartPark.Web.Comum;

namespace SmartPark.Web.Features.Administracao;

public sealed class AdministracaoApi(HttpClient http, SessaoStore sessao) : ApiClientBase(http, sessao)
{
    public Task<ApiResult<List<EstacionamentoResponse>>> ListarEstacionamentosAsync() =>
        GetAsync<List<EstacionamentoResponse>>("/api/estacionamentos?incluirInativos=true");

    public Task<ApiResult<EstacionamentoResponse>> ObterEstacionamentoAsync(Guid estacionamentoId) =>
        GetAsync<EstacionamentoResponse>($"/api/estacionamentos/{estacionamentoId}");

    public Task<ApiResult<EstacionamentoResponse>> CriarEstacionamentoAsync(EstacionamentoRequest request) =>
        PostAsync<EstacionamentoResponse>("/api/admin/estacionamentos", request);

    public Task<ApiResult<EstacionamentoResponse>> AtualizarEstacionamentoAsync(Guid estacionamentoId, EstacionamentoRequest request) =>
        PutAsync<EstacionamentoResponse>($"/api/admin/estacionamentos/{estacionamentoId}", request);

    public Task<ApiResult> AlterarSituacaoAsync(Guid estacionamentoId, bool ativo) =>
        EnviarSemRetornoAsync(HttpMethod.Put, $"/api/admin/estacionamentos/{estacionamentoId}/situacao", new AlterarSituacaoEstacionamentoRequest(ativo));

    public Task<ApiResult<OcupacaoResponse>> OcupacaoAsync(Guid estacionamentoId) =>
        GetAsync<OcupacaoResponse>($"/api/estacionamentos/{estacionamentoId}/ocupacao");

    public Task<ApiResult<List<VagaResponse>>> ListarVagasAsync(Guid estacionamentoId) =>
        GetAsync<List<VagaResponse>>($"/api/admin/estacionamentos/{estacionamentoId}/vagas");

    public Task<ApiResult<List<VagaResponse>>> CriarVagasAsync(Guid estacionamentoId, CriarVagasEmLoteRequest request) =>
        PostAsync<List<VagaResponse>>($"/api/admin/estacionamentos/{estacionamentoId}/vagas", request);

    public Task<ApiResult<VagaResponse>> AtualizarStatusVagaAsync(Guid estacionamentoId, Guid vagaId, string status) =>
        PutAsync<VagaResponse>($"/api/admin/estacionamentos/{estacionamentoId}/vagas/{vagaId}/status", new AtualizarStatusVagaRequest(status));

    public Task<ApiResult> RemoverVagaAsync(Guid estacionamentoId, Guid vagaId) =>
        EnviarSemRetornoAsync(HttpMethod.Delete, $"/api/admin/estacionamentos/{estacionamentoId}/vagas/{vagaId}");

    public Task<ApiResult<List<UsuarioResponse>>> ListarUsuariosAsync() =>
        GetAsync<List<UsuarioResponse>>("/api/admin/usuarios");

    public Task<ApiResult<UsuarioResponse>> CriarUsuarioAsync(CriarUsuarioOperacionalRequest request) =>
        PostAsync<UsuarioResponse>("/api/admin/usuarios", request);
}
