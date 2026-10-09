namespace SmartPark.Infrastructure.Autenticacao;

/// <summary>Nomes curtos das claims do token (sem o mapeamento para URIs do WS-Federation).</summary>
public static class ClaimsSmartPark
{
    public const string UsuarioId = "sub";
    public const string Nome = "name";
    public const string Email = "email";
    public const string Perfil = "role";
}
