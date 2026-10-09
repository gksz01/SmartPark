using FluentValidation;

namespace SmartPark.Application.Features.Estacionamentos.AtualizarEstacionamento;

public sealed class AtualizarEstacionamentoValidator : AbstractValidator<AtualizarEstacionamentoCommand>
{
    public AtualizarEstacionamentoValidator() => Include(new DadosEstacionamentoValidator());
}
