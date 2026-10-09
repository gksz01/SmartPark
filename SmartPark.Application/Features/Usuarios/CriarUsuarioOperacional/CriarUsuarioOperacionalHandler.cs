using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Application.Common;
using SmartPark.Application.Features.Autenticacao;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Application.Features.Usuarios.CriarUsuarioOperacional;

public sealed class CriarUsuarioOperacionalHandler(IApplicationDbContext db, IIdentityService identityService)
    : ICommandHandler<CriarUsuarioOperacionalCommand, Result<UsuarioResponse>>
{
    public async ValueTask<Result<UsuarioResponse>> Handle(CriarUsuarioOperacionalCommand command, CancellationToken cancellationToken)
    {
        var perfil = EnumTexto.Converter<Perfil>(command.Perfil);
        if (perfil == Perfil.Motorista)
        {
            return UsuarioErrors.PerfilNaoOperacional;
        }

        var email = Usuario.NormalizarEmail(command.Email);
        if (await db.Usuarios.AnyAsync(u => u.Email == email, cancellationToken))
        {
            return UsuarioErrors.EmailEmUso;
        }

        var usuario = Usuario.Criar(command.Nome, email, perfil);
        db.Usuarios.Add(usuario);

        var credenciais = await identityService.CriarCredenciaisAsync(usuario.Id, email, command.Senha, cancellationToken);

        return credenciais.IsFailure
            ? credenciais.Error
            : usuario.ParaResponse();
    }
}
