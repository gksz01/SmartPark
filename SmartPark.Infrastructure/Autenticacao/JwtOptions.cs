namespace SmartPark.Infrastructure.Autenticacao;

public sealed class JwtOptions
{
    public const string Secao = "Jwt";

    public string Issuer { get; set; } = "smartpark";

    public string Audience { get; set; } = "smartpark-clients";

    /// <summary>Chave HMAC-SHA256, com pelo menos 32 bytes. Vem de segredo do AppHost, nunca do código.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpiracaoMinutos { get; set; } = 60;
}
