using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Application.Features.Autenticacao;

internal static class UsuarioMapeamento
{
    public static UsuarioResponse ParaResponse(this Usuario usuario) =>
        new(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil.ToString(), usuario.Ativo);

    public static TokenResponse ParaTokenResponse(this TokenGerado token, Usuario usuario) =>
        new(token.AccessToken, token.ExpiraEm, usuario.ParaResponse());
}
