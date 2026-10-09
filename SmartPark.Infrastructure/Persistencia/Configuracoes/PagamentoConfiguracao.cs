using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPark.Domain.Pagamentos;
using SmartPark.Domain.Reservas;

namespace SmartPark.Infrastructure.Persistencia.Configuracoes;

internal sealed class PagamentoConfiguracao : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("pagamentos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Valor).HasPrecision(10, 2);
        builder.Property(p => p.Metodo).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.TransacaoExternaId).HasMaxLength(100);
        builder.Property(p => p.MotivoRecusa).HasMaxLength(300);

        // Uma reserva tem no máximo um pagamento (as novas tentativas reaproveitam o registro).
        builder.HasOne<Reserva>().WithOne().HasForeignKey<Pagamento>(p => p.ReservaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(p => p.ReservaId).IsUnique();
    }
}
