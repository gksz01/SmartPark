using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Autenticacao.RegistrarMotorista;
using SmartPark.Contracts.Autenticacao;

namespace SmartPark.Api.Features.Autenticacao;

public sealed class RegistrarMotoristaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/api/auth/registro", async (RegistrarMotoristaRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new RegistrarMotoristaCommand(request.Nome, request.Email, request.Senha), cancellationToken);
                return resultado.ToCreated(_ => "/api/auth/me");
            })
            .AllowAnonymous()
            .WithTags(Tags.Autenticacao)
            .WithSummary("Cadastra um motorista e devolve o token de acesso (RF01).")
            .Produces<TokenResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
}
