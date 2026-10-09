using Mediator;
using SmartPark.Api.Configuracao;
using SmartPark.Application.Features.Autenticacao.Login;
using SmartPark.Contracts.Autenticacao;

namespace SmartPark.Api.Features.Autenticacao;

public sealed class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/api/auth/login", async (LoginRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var resultado = await sender.Send(new LoginCommand(request.Email, request.Senha), cancellationToken);
                return resultado.ToOk();
            })
            .AllowAnonymous()
            .WithTags(Tags.Autenticacao)
            .WithSummary("Autentica e devolve o token de acesso com o perfil do usuário (RF02).")
            .Produces<TokenResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
}
