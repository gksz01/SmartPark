using FluentValidation;

namespace SmartPark.Application.Features.Estacionamentos.CriarEstacionamento;

public sealed class CriarEstacionamentoValidator : AbstractValidator<CriarEstacionamentoCommand>
{
    public CriarEstacionamentoValidator() => Include(new DadosEstacionamentoValidator());
}
