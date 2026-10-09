using SmartPark.Domain.Common;

namespace SmartPark.Domain.Usuarios;

/// <summary>
/// Perfil de negócio de quem usa o SmartPark. As credenciais ficam no provedor de identidade, que
/// compartilha o mesmo Id.
/// </summary>
public sealed class Usuario : AuditableEntity
{
    private Usuario()
    {
    }

    public string Nome { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public Perfil Perfil { get; private set; }

    public bool Ativo { get; private set; }

    public static Usuario Criar(string nome, string email, Perfil perfil) => new()
    {
        Nome = nome.Trim(),
        Email = NormalizarEmail(email),
        Perfil = perfil,
        Ativo = true
    };

    public static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
}
