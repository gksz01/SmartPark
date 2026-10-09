using FluentValidation;

namespace SmartPark.Application.Features.Veiculos.AtualizarVeiculo;

public sealed class AtualizarVeiculoValidator : AbstractValidator<AtualizarVeiculoCommand>
{
    public AtualizarVeiculoValidator()
    {
        RuleFor(c => c.Placa).PlacaValida();
        RuleFor(c => c.Modelo).ModeloValido();
    }
}
