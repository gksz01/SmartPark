using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Usuarios.ListarUsuarios;
using SmartPark.Contracts.Autenticacao;

namespace SmartPark.Api.Features.Usuarios;

public sealed class ListarUsuariosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/admin/usuarios", async (string? perfil, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ListarUsuariosQuery(perfil), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization(Politicas.Administrador)
            .WithTags(Tags.Usuarios)
            .WithSummary("Lista os usuários, com filtro opcional por perfil.")
            .Produces<IReadOnlyList<UsuarioResponse>>();
}
