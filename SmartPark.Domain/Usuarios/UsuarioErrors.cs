using SmartPark.Domain.Common;

namespace SmartPark.Domain.Usuarios;

public static class UsuarioErrors
{
    public static readonly Error EmailEmUso =
        Error.Conflict("Usuario.EmailEmUso", "Já existe um usuário cadastrado com este e-mail.");

    public static readonly Error CredenciaisInvalidas =
        Error.Unauthorized("Usuario.CredenciaisInvalidas", "E-mail ou senha inválidos.");

    public static readonly Error Bloqueado =
        Error.Unauthorized("Usuario.Bloqueado", "Conta temporariamente bloqueada por excesso de tentativas. Tente novamente mais tarde.");

    public static readonly Error Inativo =
        Error.Forbidden("Usuario.Inativo", "Este usuário está inativo.");

    public static readonly Error NaoEncontrado =
        Error.NotFound("Usuario.NaoEncontrado", "Usuário não encontrado.");

    public static readonly Error PerfilNaoOperacional =
        Error.Validation("Usuario.PerfilNaoOperacional", "Usuários operacionais devem ser Funcionário ou Administrador.");

    public static Error SenhaInvalida(string detalhes) =>
        Error.Validation("Usuario.SenhaInvalida", detalhes);
}
