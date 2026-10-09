using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SmartPark.Api.Configuracao;

/// <summary>Declara o JWT no documento OpenAPI, para que o Scalar consiga enviar o token.</summary>
internal sealed class BearerSecurityTransformer : IOpenApiDocumentTransformer
{
    private const string Esquema = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            [Esquema] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "JWT",
                Description = "Token obtido em POST /api/auth/login."
            }
        };

        document.Security = [new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference(Esquema, document)] = [] }];

        return Task.CompletedTask;
    }
}
