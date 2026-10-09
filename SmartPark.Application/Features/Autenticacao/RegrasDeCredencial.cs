using FluentValidation;

namespace SmartPark.Application.Features.Autenticacao;

/// <summary>Regras de formato comuns ao cadastro de motoristas e de usuários operacionais.</summary>
internal static class RegrasDeCredencial
{
    public const int TamanhoMinimoSenha = 8;

    public static IRuleBuilderOptions<T, string> NomeDeUsuario<T>(this IRuleBuilder<T, string> regra) =>
        regra.NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

    public static IRuleBuilderOptions<T, string> EmailDeUsuario<T>(this IRuleBuilder<T, string> regra) =>
        regra.NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Informe um e-mail válido.")
            .MaximumLength(256).WithMessage("O e-mail deve ter no máximo 256 caracteres.");

    public static IRuleBuilderOptions<T, string> SenhaDeUsuario<T>(this IRuleBuilder<T, string> regra) =>
        regra.NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(TamanhoMinimoSenha).WithMessage($"A senha deve ter pelo menos {TamanhoMinimoSenha} caracteres.")
            .Matches("[A-Z]").WithMessage("A senha deve conter uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("A senha deve conter uma letra minúscula.")
            .Matches("[0-9]").WithMessage("A senha deve conter um número.");
}
