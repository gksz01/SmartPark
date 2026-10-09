using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartPark.Domain.Common;

namespace SmartPark.Infrastructure.Persistencia;

/// <summary>Preenche CriadoEm e AtualizadoEm de toda <see cref="AuditableEntity"/> no momento da gravação.</summary>
public sealed class AuditoriaInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Carimbar(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Carimbar(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Carimbar(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var agora = timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(e => e.CriadoEm).CurrentValue = agora;
                    break;
                case EntityState.Modified:
                    entry.Property(e => e.AtualizadoEm).CurrentValue = agora;
                    break;
            }
        }
    }
}
