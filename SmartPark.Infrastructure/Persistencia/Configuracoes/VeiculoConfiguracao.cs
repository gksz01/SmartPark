using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPark.Domain.Usuarios;
using SmartPark.Domain.Veiculos;

namespace SmartPark.Infrastructure.Persistencia.Configuracoes;

internal sealed class VeiculoConfiguracao : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("veiculos");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();

        builder.Property(v => v.Placa)
            .HasConversion(placa => placa.Valor, valor => Placa.Criar(valor).Value)
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(v => v.Modelo).HasMaxLength(100).IsRequired();

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(v => v.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Busca por placa: unicidade entre veículos ativos e, na fase 2, leitura na cancela.
        builder.HasIndex(v => v.Placa);
    }
}
