using FluentValidation;

namespace SmartPark.Application.Features.Autenticacao.Login;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(c => c.Email).NotEmpty().WithMessage("O e-mail é obrigatório.");
        RuleFor(c => c.Senha).NotEmpty().WithMessage("A senha é obrigatória.");
    }
}
