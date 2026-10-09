using SmartPark.Domain.Common;

namespace SmartPark.Application.Abstractions.Autenticacao;

/// <summary>Credenciais (senha, bloqueio por tentativas). O perfil de negócio fica em <c>Usuario</c>.</summary>
public interface IIdentityService
{
    /// <summary>
    /// Cria as credenciais para o usuário de mesmo Id e grava a unidade de trabalho inteira — inclusive o
    /// <c>Usuario</c> adicionado antes —, de modo que perfil e credencial nascem juntos ou não nascem.
    /// </summary>
    Task<Result> CriarCredenciaisAsync(Guid usuarioId, string email, string senha, CancellationToken cancellationToken);

    /// <summary>Confere e-mail e senha, aplicando o bloqueio por tentativas. Devolve o Id do usuário.</summary>
    Task<Result<Guid>> VerificarCredenciaisAsync(string email, string senha, CancellationToken cancellationToken);
}
