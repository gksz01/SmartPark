using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Infrastructure.Persistencia.Configuracoes;

internal sealed class EstacionamentoConfiguracao : IEntityTypeConfiguration<Estacionamento>
{
    public void Configure(EntityTypeBuilder<Estacionamento> builder)
    {
        builder.ToTable("estacionamentos");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Nome).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Endereco).HasMaxLength(300).IsRequired();
        builder.Property(e => e.ValorHora).HasPrecision(10, 2);

        // Concorrência otimista na confirmação de reservas (ver Estacionamento.GarantirCapacidade).
        builder.Property(e => e.Versao).IsConcurrencyToken();

        builder.HasMany(e => e.Vagas)
            .WithOne()
            .HasForeignKey(v => v.EstacionamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Vagas).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
