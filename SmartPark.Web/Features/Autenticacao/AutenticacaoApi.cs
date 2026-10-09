using SmartPark.Contracts.Autenticacao;
using SmartPark.Web.Autenticacao;
using SmartPark.Web.Comum;

namespace SmartPark.Web.Features.Autenticacao;

public sealed class AutenticacaoApi(HttpClient http, SessaoStore sessao) : ApiClientBase(http, sessao)
{
    public Task<ApiResult<TokenResponse>> LoginAsync(LoginRequest request) =>
        PostAsync<TokenResponse>("/api/auth/login", request);

    public Task<ApiResult<TokenResponse>> RegistrarAsync(RegistrarMotoristaRequest request) =>
        PostAsync<TokenResponse>("/api/auth/registro", request);
}
