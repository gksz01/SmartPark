using FluentValidation;

namespace SmartPark.Application.Features.Estacionamentos.ListarEstacionamentos;

public sealed class ListarEstacionamentosValidator : AbstractValidator<ListarEstacionamentosQuery>
{
    public ListarEstacionamentosValidator()
    {
        RuleFor(q => q.Busca).MaximumLength(100).WithMessage("A busca deve ter no máximo 100 caracteres.");
        RuleFor(q => q.Latitude).InclusiveBetween(-90d, 90d).WithMessage("A latitude deve estar entre -90 e 90.");
        RuleFor(q => q.Longitude).InclusiveBetween(-180d, 180d).WithMessage("A longitude deve estar entre -180 e 180.");
        RuleFor(q => q)
            .Must(q => q.Latitude.HasValue == q.Longitude.HasValue)
            .WithName("Coordenadas")
            .WithMessage("Informe latitude e longitude juntas.");
    }
}
