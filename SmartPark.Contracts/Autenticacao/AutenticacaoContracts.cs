namespace SmartPark.Contracts.Autenticacao;

public sealed record RegistrarMotoristaRequest(string Nome, string Email, string Senha);

public sealed record LoginRequest(string Email, string Senha);

public sealed record UsuarioResponse(Guid Id, string Nome, string Email, string Perfil, bool Ativo);

public sealed record TokenResponse(string AccessToken, DateTimeOffset ExpiraEm, UsuarioResponse Usuario);
