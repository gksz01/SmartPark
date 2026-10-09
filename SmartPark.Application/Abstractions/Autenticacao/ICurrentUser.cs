namespace SmartPark.Application.Abstractions.Autenticacao;

/// <summary>Usuário autenticado na requisição atual.</summary>
public interface ICurrentUser
{
    Guid UsuarioId { get; }

    bool EhAdministrador { get; }
}
