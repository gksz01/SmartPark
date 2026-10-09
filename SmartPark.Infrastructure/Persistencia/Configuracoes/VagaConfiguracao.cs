using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Infrastructure.Persistencia.Configuracoes;

internal sealed class VagaConfiguracao : IEntityTypeConfiguration<Vaga>
{
    public void Configure(EntityTypeBuilder<Vaga> builder)
    {
        builder.ToTable("vagas");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();

        builder.Property(v => v.Identificador).HasMaxLength(20).IsRequired();
        builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(v => new { v.EstacionamentoId, v.Identificador }).IsUnique();
    }
}
