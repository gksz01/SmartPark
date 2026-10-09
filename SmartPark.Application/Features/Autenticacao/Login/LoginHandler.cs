using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Application.Features.Autenticacao.Login;

public sealed class LoginHandler(
    IApplicationDbContext db,
    IIdentityService identityService,
    ITokenService tokenService)
    : ICommandHandler<LoginCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var credenciais = await identityService.VerificarCredenciaisAsync(
            Usuario.NormalizarEmail(command.Email), command.Senha, cancellationToken);

        if (credenciais.IsFailure)
        {
            return credenciais.Error;
        }

        var usuario = await db.Usuarios.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == credenciais.Value, cancellationToken);

        if (usuario is null)
        {
            return UsuarioErrors.CredenciaisInvalidas;
        }

        if (!usuario.Ativo)
        {
            return UsuarioErrors.Inativo;
        }

        return tokenService.Gerar(usuario).ParaTokenResponse(usuario);
    }
}
