using SmartPark.Domain.Usuarios;

namespace SmartPark.Application.Abstractions.Autenticacao;

public interface ITokenService
{
    TokenGerado Gerar(Usuario usuario);
}

public sealed record TokenGerado(string AccessToken, DateTimeOffset ExpiraEm);
