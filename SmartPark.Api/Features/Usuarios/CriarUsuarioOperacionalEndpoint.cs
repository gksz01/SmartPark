using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Usuarios.CriarUsuarioOperacional;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Contracts.Usuarios;

namespace SmartPark.Api.Features.Usuarios;

public sealed class CriarUsuarioOperacionalEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/api/admin/usuarios", async (CriarUsuarioOperacionalRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(
                    new CriarUsuarioOperacionalCommand(request.Nome, request.Email, request.Senha, request.Perfil),
                    cancellationToken);

                return resultado.ToCreated(u => $"/api/admin/usuarios/{u.Id}");
            })
            .RequireAuthorization(Politicas.Administrador)
            .WithTags(Tags.Usuarios)
            .WithSummary("Cria um funcionário ou administrador (RF24).")
            .Produces<UsuarioResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
