namespace SmartPark.MigrationService.Seed;

public sealed class SeedOptions
{
    public const string Secao = "Seed";

    public string AdminNome { get; set; } = "Administrador";

    public string AdminEmail { get; set; } = "admin@smartpark.local";

    /// <summary>Vem do parâmetro secreto do AppHost. Sem ela, o administrador inicial não é criado.</summary>
    public string? AdminPassword { get; set; }

    /// <summary>Estacionamentos de demonstração e um motorista de teste. Ligado só no desenvolvimento.</summary>
    public bool DadosDemo { get; set; }
}
