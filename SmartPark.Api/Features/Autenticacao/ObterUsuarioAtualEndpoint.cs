using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Autenticacao.ObterUsuarioAtual;
using SmartPark.Contracts.Autenticacao;

namespace SmartPark.Api.Features.Autenticacao;

public sealed class ObterUsuarioAtualEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/api/auth/me", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new ObterUsuarioAtualQuery(), cancellationToken);
                return resultado.ToOk();
            })
            .RequireAuthorization()
            .WithTags(Tags.Autenticacao)
            .WithSummary("Dados do usuário autenticado.")
            .Produces<UsuarioResponse>();
}
