using FluentValidation;

namespace SmartPark.Application.Features.Autenticacao.RegistrarMotorista;

public sealed class RegistrarMotoristaValidator : AbstractValidator<RegistrarMotoristaCommand>
{
    public RegistrarMotoristaValidator()
    {
        RuleFor(c => c.Nome).NomeDeUsuario();
        RuleFor(c => c.Email).EmailDeUsuario();
        RuleFor(c => c.Senha).SenhaDeUsuario();
    }
}
