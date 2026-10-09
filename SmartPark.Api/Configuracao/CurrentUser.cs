using System.Security.Claims;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Infrastructure.Autenticacao;

namespace SmartPark.Api.Configuracao;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UsuarioId =>
        Guid.TryParse(Principal.FindFirstValue(ClaimsSmartPark.UsuarioId), out var id)
            ? id
            : throw new InvalidOperationException("Não há usuário autenticado nesta requisição.");

    public bool EhAdministrador => Principal.IsInRole(Perfis.Administrador);

    private ClaimsPrincipal Principal =>
        httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("Não há requisição HTTP em andamento.");
}
