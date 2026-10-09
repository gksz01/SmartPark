using FluentValidation;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Vagas.CriarVagasEmLote;

public sealed class CriarVagasEmLoteValidator : AbstractValidator<CriarVagasEmLoteCommand>
{
    public CriarVagasEmLoteValidator()
    {
        RuleFor(c => c.Prefixo)
            .NotEmpty().WithMessage("O prefixo é obrigatório.")
            .Matches("^[A-Za-z0-9]{1,5}$").WithMessage(EstacionamentoErrors.PrefixoInvalido.Message);

        RuleFor(c => c.Quantidade)
            .InclusiveBetween(1, Estacionamento.MaximoVagasPorLote)
            .WithMessage(EstacionamentoErrors.QuantidadeVagasInvalida.Message);
    }
}
