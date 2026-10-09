using FluentValidation;
using SmartPark.Application.Common;
using SmartPark.Domain.Pagamentos;

namespace SmartPark.Application.Features.Pagamentos.PagarReserva;

public sealed class PagarReservaValidator : AbstractValidator<PagarReservaCommand>
{
    public PagarReservaValidator()
    {
        RuleFor(c => c.Metodo)
            .Must(EnumTexto.EhValido<MetodoPagamento>)
            .WithMessage($"Método de pagamento inválido. Opções: {EnumTexto.Opcoes<MetodoPagamento>()}.");

        RuleFor(c => c.NumeroCartao)
            .NotEmpty().WithMessage("Informe o número do cartão.")
            .Matches("^[0-9]{13,19}$").WithMessage("O número do cartão deve ter de 13 a 19 dígitos.")
            .When(c => string.Equals(c.Metodo, nameof(MetodoPagamento.Cartao), StringComparison.OrdinalIgnoreCase));
    }
}
