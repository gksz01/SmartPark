using FluentValidation;
using SmartPark.Contracts.Veiculos;
using SmartPark.Domain.Veiculos;

namespace SmartPark.Application.Features.Veiculos;

internal static class VeiculoMapeamento
{
    public static VeiculoResponse ParaResponse(this Veiculo veiculo) =>
        new(veiculo.Id, veiculo.Placa.Valor, veiculo.Modelo);

    public static IRuleBuilderOptions<T, string> PlacaValida<T>(this IRuleBuilder<T, string> regra) =>
        regra.Must(p => Placa.Criar(p).IsSuccess)
            .WithMessage(VeiculoErrors.PlacaInvalida.Message);

    public static IRuleBuilderOptions<T, string> ModeloValido<T>(this IRuleBuilder<T, string> regra) =>
        regra.NotEmpty().WithMessage("O modelo é obrigatório.")
            .MaximumLength(100).WithMessage("O modelo deve ter no máximo 100 caracteres.");
}
