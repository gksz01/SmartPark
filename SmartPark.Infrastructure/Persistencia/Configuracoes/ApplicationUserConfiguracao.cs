using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPark.Domain.Usuarios;
using SmartPark.Infrastructure.Identidade;

namespace SmartPark.Infrastructure.Persistencia.Configuracoes;

internal sealed class ApplicationUserConfiguracao : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // A credencial compartilha o Id do perfil de negócio: um não existe sem o outro.
        builder.HasOne<Usuario>()
            .WithOne()
            .HasForeignKey<ApplicationUser>(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
