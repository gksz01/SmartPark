using Microsoft.AspNetCore.Identity;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Domain.Common;
using SmartPark.Domain.Usuarios;

namespace SmartPark.Infrastructure.Identidade;

internal sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<Result> CriarCredenciaisAsync(Guid usuarioId, string email, string senha, CancellationToken cancellationToken)
    {
        var usuario = new ApplicationUser
        {
            Id = usuarioId,
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        // O UserStore grava pelo mesmo DbContext da requisição, então o Usuario de domínio já adicionado
        // vai junto, na mesma transação.
        var resultado = await userManager.CreateAsync(usuario, senha);
        if (resultado.Succeeded)
        {
            return Result.Success();
        }

        if (resultado.Errors.Any(e => e.Code is nameof(IdentityErrorDescriber.DuplicateEmail) or nameof(IdentityErrorDescriber.DuplicateUserName)))
        {
            return UsuarioErrors.EmailEmUso;
        }

        return UsuarioErrors.SenhaInvalida(string.Join(" ", resultado.Errors.Select(e => e.Description)));
    }

    public async Task<Result<Guid>> VerificarCredenciaisAsync(string email, string senha, CancellationToken cancellationToken)
    {
        var usuario = await userManager.FindByEmailAsync(email);
        if (usuario is null)
        {
            return UsuarioErrors.CredenciaisInvalidas;
        }

        if (await userManager.IsLockedOutAsync(usuario))
        {
            return UsuarioErrors.Bloqueado;
        }

        if (!await userManager.CheckPasswordAsync(usuario, senha))
        {
            await userManager.AccessFailedAsync(usuario);

            return await userManager.IsLockedOutAsync(usuario)
                ? UsuarioErrors.Bloqueado
                : UsuarioErrors.CredenciaisInvalidas;
        }

        await userManager.ResetAccessFailedCountAsync(usuario);
        return usuario.Id;
    }
}
