using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Infrastructure.Autenticacao;

internal sealed class TokenService(IOptions<JwtOptions> options, TimeProvider timeProvider) : ITokenService
{
    private readonly JsonWebTokenHandler _handler = new();

    public TokenGerado Gerar(Usuario usuario)
    {
        var jwt = options.Value;
        var agora = timeProvider.GetUtcNow();
        var expiraEm = agora.AddMinutes(jwt.ExpiracaoMinutos);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            IssuedAt = agora.UtcDateTime,
            NotBefore = agora.UtcDateTime,
            Expires = expiraEm.UtcDateTime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [ClaimsSmartPark.UsuarioId] = usuario.Id.ToString(),
                [ClaimsSmartPark.Nome] = usuario.Nome,
                [ClaimsSmartPark.Email] = usuario.Email,
                [ClaimsSmartPark.Perfil] = usuario.Perfil.ToString()
            }
        };

        return new TokenGerado(_handler.CreateToken(descriptor), expiraEm);
    }
}
