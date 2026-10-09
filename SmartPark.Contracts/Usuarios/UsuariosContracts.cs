namespace SmartPark.Contracts.Usuarios;

/// <param name="Perfil">Funcionario ou Administrador.</param>
public sealed record CriarUsuarioOperacionalRequest(string Nome, string Email, string Senha, string Perfil);
