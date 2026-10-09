using FluentValidation;
using SmartPark.Application.Features.Autenticacao;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Application.Features.Usuarios.CriarUsuarioOperacional;

public sealed class CriarUsuarioOperacionalValidator : AbstractValidator<CriarUsuarioOperacionalCommand>
{
    private static readonly string[] PerfisOperacionais = [nameof(Perfil.Funcionario), nameof(Perfil.Administrador)];

    public CriarUsuarioOperacionalValidator()
    {
        RuleFor(c => c.Nome).NomeDeUsuario();
        RuleFor(c => c.Email).EmailDeUsuario();
        RuleFor(c => c.Senha).SenhaDeUsuario();
        RuleFor(c => c.Perfil)
            .Must(p => PerfisOperacionais.Contains(p, StringComparer.OrdinalIgnoreCase))
            .WithMessage("O perfil deve ser Funcionario ou Administrador.");
    }
}
