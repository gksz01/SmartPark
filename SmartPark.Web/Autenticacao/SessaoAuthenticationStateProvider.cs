using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using SmartPark.Contracts.Autenticacao;

namespace SmartPark.Web.Autenticacao;

/// <summary>
/// Estado de autenticação do Blazor montado a partir da sessão. Serve só para a interface (menus e
/// páginas por perfil); quem decide de verdade o que cada perfil pode fazer é a API.
/// </summary>
public sealed class SessaoAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private static readonly AuthenticationState Anonimo = new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly SessaoStore _sessao;

    public SessaoAuthenticationStateProvider(SessaoStore sessao)
    {
        _sessao = sessao;
        _sessao.SessaoEncerrada += AoEncerrarSessao;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var sessao = await _sessao.ObterAsync();
        return sessao is null ? Anonimo : Estado(sessao.Usuario);
    }

    public async Task EntrarAsync(TokenResponse token)
    {
        await _sessao.SalvarAsync(token);
        NotifyAuthenticationStateChanged(Task.FromResult(Estado(token.Usuario)));
    }

    public Task SairAsync() => _sessao.EncerrarAsync();

    public void Dispose() => _sessao.SessaoEncerrada -= AoEncerrarSessao;

    private void AoEncerrarSessao() => NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));

    private static AuthenticationState Estado(UsuarioResponse usuario)
    {
        var identidade = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Perfil)
            ],
            authenticationType: "SmartPark");

        return new AuthenticationState(new ClaimsPrincipal(identidade));
    }
}
