using FluentValidation;
using SmartPark.Domain.Reservas;

namespace SmartPark.Application.Features.Reservas.CriarReserva;

public sealed class CriarReservaValidator : AbstractValidator<CriarReservaCommand>
{
    public CriarReservaValidator()
    {
        RuleFor(c => c.EstacionamentoId).NotEmpty().WithMessage("Selecione o estacionamento.");
        RuleFor(c => c.VeiculoId).NotEmpty().WithMessage("Selecione o veículo.");
        RuleFor(c => c.Fim).GreaterThan(c => c.Inicio).WithMessage(ReservaErrors.FimAntesDoInicio.Message);
    }
}
