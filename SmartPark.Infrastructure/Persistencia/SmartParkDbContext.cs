using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Domain.Estacionamentos;
using SmartPark.Domain.Pagamentos;
using SmartPark.Domain.Reservas;
using SmartPark.Domain.Usuarios;
using SmartPark.Domain.Veiculos;
using SmartPark.Infrastructure.Identidade;

namespace SmartPark.Infrastructure.Persistencia;

/// <summary>
/// Contexto único do SmartPark. Inclui só as tabelas de usuário do Identity: os perfis (Motorista,
/// Funcionário, Administrador) são do domínio, em <see cref="Usuario.Perfil"/>, e não roles do Identity.
/// </summary>
public sealed class SmartParkDbContext(DbContextOptions<SmartParkDbContext> options)
    : IdentityUserContext<ApplicationUser, Guid>(options), IApplicationDbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Veiculo> Veiculos => Set<Veiculo>();

    public DbSet<Estacionamento> Estacionamentos => Set<Estacionamento>();

    public DbSet<Vaga> Vagas => Set<Vaga>();

    public DbSet<Reserva> Reservas => Set<Reserva>();

    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(SmartParkDbContext).Assembly);
    }
}
