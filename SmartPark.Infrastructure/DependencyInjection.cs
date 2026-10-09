using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Application.Abstractions.Pagamentos;
using SmartPark.Infrastructure.Autenticacao;
using SmartPark.Infrastructure.Identidade;
using SmartPark.Infrastructure.Pagamentos;
using SmartPark.Infrastructure.Persistencia;

namespace SmartPark.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Tudo o que a API precisa da infraestrutura.</summary>
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddPersistencia();
        builder.Services.AddIdentidade();
        builder.Services.AddAutenticacaoJwt(builder.Configuration);

        builder.Services.AddOptions<GatewayPagamentoOptions>().Bind(builder.Configuration.GetSection(GatewayPagamentoOptions.Secao));
        builder.Services.AddScoped<IGatewayPagamento, GatewayPagamentoSimulado>();

        return builder;
    }

    /// <summary>
    /// Registra o DbContext no provider escolhido em <c>Database:Provider</c> (PostgreSql por padrão) e
    /// liga a integração do Aspire (retentativas, health check e telemetria) do provider correspondente.
    /// </summary>
    public static IHostApplicationBuilder AddPersistencia(this IHostApplicationBuilder builder)
    {
        var provedor = builder.Configuration.GetValue(BancoDeDados.SecaoProvider, ProvedorBancoDados.PostgreSql);
        var connectionString = builder.Configuration.GetConnectionString(BancoDeDados.NomeConexao);

        builder.Services.TryAddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<AuditoriaInterceptor>();

        builder.Services.AddDbContext<SmartParkDbContext>((serviceProvider, options) =>
        {
            BancoDeDados.Configurar(options, provedor, connectionString);
            options.AddInterceptors(serviceProvider.GetRequiredService<AuditoriaInterceptor>());
        });

        if (provedor == ProvedorBancoDados.SqlServer)
        {
            builder.EnrichSqlServerDbContext<SmartParkDbContext>();
        }
        else
        {
            builder.EnrichNpgsqlDbContext<SmartParkDbContext>();
        }

        builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<SmartParkDbContext>());

        return builder;
    }

    public static IServiceCollection AddIdentidade(this IServiceCollection services)
    {
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddErrorDescriber<IdentityErrorDescriberPtBr>()
            .AddEntityFrameworkStores<SmartParkDbContext>();

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }

    public static IServiceCollection AddAutenticacaoJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.Secao))
            .Validate(o => Encoding.UTF8.GetByteCount(o.Key) >= 32, "Jwt:Key deve ter pelo menos 32 bytes.")
            .ValidateOnStart();

        services.AddSingleton<ITokenService, TokenService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                // Mantém as claims com os nomes curtos do token (sub, name, role).
                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    NameClaimType = ClaimsSmartPark.Nome,
                    RoleClaimType = ClaimsSmartPark.Perfil,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        return services;
    }
}
