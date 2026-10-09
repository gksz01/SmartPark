using FluentValidation;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Estacionamentos;

internal static class EstacionamentoMapeamento
{
    public static EstacionamentoResponse ParaResponse(this Estacionamento estacionamento, int totalVagas, double? distanciaKm = null) =>
        new(
            estacionamento.Id,
            estacionamento.Nome,
            estacionamento.Endereco,
            estacionamento.Latitude,
            estacionamento.Longitude,
            estacionamento.CapacidadeTotal,
            estacionamento.CapacidadeReservavel,
            estacionamento.ValorHora,
            estacionamento.Ativo,
            totalVagas,
            distanciaKm);
}

/// <summary>Regras de formato do cadastro; as invariantes de negócio são garantidas pelo agregado.</summary>
internal sealed class DadosEstacionamentoValidator : AbstractValidator<IDadosEstacionamento>
{
    public DadosEstacionamentoValidator()
    {
        RuleFor(d => d.Nome).NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");
        RuleFor(d => d.Endereco).NotEmpty().WithMessage("O endereço é obrigatório.")
            .MaximumLength(300).WithMessage("O endereço deve ter no máximo 300 caracteres.");
        RuleFor(d => d.Latitude).InclusiveBetween(-90d, 90d).WithMessage("A latitude deve estar entre -90 e 90.");
        RuleFor(d => d.Longitude).InclusiveBetween(-180d, 180d).WithMessage("A longitude deve estar entre -180 e 180.");
        RuleFor(d => d)
            .Must(d => d.Latitude.HasValue == d.Longitude.HasValue)
            .WithName("Coordenadas")
            .WithMessage("Informe latitude e longitude juntas, ou nenhuma delas.");
        RuleFor(d => d.CapacidadeTotal).InclusiveBetween(1, 10_000).WithMessage("A capacidade total deve estar entre 1 e 10.000.");
        RuleFor(d => d.CapacidadeReservavel)
            .GreaterThanOrEqualTo(0).WithMessage("A capacidade reservável não pode ser negativa.")
            .LessThanOrEqualTo(d => d.CapacidadeTotal).WithMessage("A capacidade reservável não pode ser maior que a capacidade total.");
        RuleFor(d => d.ValorHora).InclusiveBetween(0m, 10_000m).WithMessage("O valor por hora deve estar entre 0 e 10.000.");
    }
}

internal interface IDadosEstacionamento
{
    string Nome { get; }

    string Endereco { get; }

    double? Latitude { get; }

    double? Longitude { get; }

    int CapacidadeTotal { get; }

    int CapacidadeReservavel { get; }

    decimal ValorHora { get; }
}
