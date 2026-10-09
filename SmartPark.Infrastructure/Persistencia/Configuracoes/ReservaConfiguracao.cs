using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPark.Domain.Estacionamentos;
using SmartPark.Domain.Reservas;
using SmartPark.Domain.Usuarios;
using SmartPark.Domain.Veiculos;

namespace SmartPark.Infrastructure.Persistencia.Configuracoes;

internal sealed class ReservaConfiguracao : IEntityTypeConfiguration<Reserva>
{
    public void Configure(EntityTypeBuilder<Reserva> builder)
    {
        builder.ToTable("reservas");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.ValorEstimado).HasPrecision(10, 2);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Ignore(r => r.Periodo);

        builder.HasOne<Usuario>().WithMany().HasForeignKey(r => r.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Veiculo>().WithMany().HasForeignKey(r => r.VeiculoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Estacionamento>().WithMany().HasForeignKey(r => r.EstacionamentoId).OnDelete(DeleteBehavior.Restrict);

        // Consulta de capacidade: reservas de um estacionamento que se sobrepõem a um período.
        builder.HasIndex(r => new { r.EstacionamentoId, r.Inicio, r.Fim });
    }
}
