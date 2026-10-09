using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartPark.Domain.Estacionamentos;
using SmartPark.Domain.Pagamentos;
using SmartPark.Domain.Reservas;
using SmartPark.Domain.Usuarios;
using SmartPark.Domain.Veiculos;

namespace SmartPark.Application.Abstractions.Data;

/// <summary>
/// Unidade de trabalho usada pelas fatias. Os handlers consultam com LINQ direto nos DbSets — sem
/// repositórios genéricos — e a implementação concreta (provider, mapeamento) fica na infraestrutura.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Usuario> Usuarios { get; }

    DbSet<Veiculo> Veiculos { get; }

    DbSet<Estacionamento> Estacionamentos { get; }

    DbSet<Vaga> Vagas { get; }

    DbSet<Reserva> Reservas { get; }

    DbSet<Pagamento> Pagamentos { get; }

    ChangeTracker ChangeTracker { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
