using FluentValidation;
using SmartPark.Application.Common;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Application.Features.Usuarios.ListarUsuarios;

public sealed class ListarUsuariosValidator : AbstractValidator<ListarUsuariosQuery>
{
    public ListarUsuariosValidator()
    {
        RuleFor(q => q.Perfil)
            .Must(EnumTexto.EhValido<Perfil>)
            .When(q => !string.IsNullOrWhiteSpace(q.Perfil))
            .WithMessage($"Perfil inválido. Opções: {EnumTexto.Opcoes<Perfil>()}.");
    }
}
