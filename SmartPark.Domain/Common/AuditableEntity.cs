namespace SmartPark.Domain.Common;

/// <summary>
/// Entidade com carimbo de criação e alteração. Os valores são preenchidos pela persistência no
/// momento do SaveChanges, então o domínio nunca precisa lidar com o relógio para isso.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CriadoEm { get; private set; }

    public DateTimeOffset? AtualizadoEm { get; private set; }
}
