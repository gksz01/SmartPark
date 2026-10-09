using FluentValidation;
using SmartPark.Application.Common;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Vagas.AtualizarStatusVaga;

public sealed class AtualizarStatusVagaValidator : AbstractValidator<AtualizarStatusVagaCommand>
{
    public AtualizarStatusVagaValidator()
    {
        RuleFor(c => c.Status)
            .Must(EnumTexto.EhValido<StatusVaga>)
            .WithMessage($"Status inválido. Opções: {EnumTexto.Opcoes<StatusVaga>()}.");
    }
}
