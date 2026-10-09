using FluentValidation;

namespace SmartPark.Application.Features.Veiculos.CadastrarVeiculo;

public sealed class CadastrarVeiculoValidator : AbstractValidator<CadastrarVeiculoCommand>
{
    public CadastrarVeiculoValidator()
    {
        RuleFor(c => c.Placa).PlacaValida();
        RuleFor(c => c.Modelo).ModeloValido();
    }
}
