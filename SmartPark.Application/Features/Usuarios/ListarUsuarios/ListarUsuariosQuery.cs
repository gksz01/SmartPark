using Mediator;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Usuarios.ListarUsuarios;

/// <param name="Perfil">Filtro opcional por perfil.</param>
public sealed record ListarUsuariosQuery(string? Perfil) : IQuery<Result<IReadOnlyList<UsuarioResponse>>>;
