using Mediator;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Usuarios.CriarUsuarioOperacional;

public sealed record CriarUsuarioOperacionalCommand(string Nome, string Email, string Senha, string Perfil)
    : ICommand<Result<UsuarioResponse>>;
