using SmartPark.Domain.Common;

namespace SmartPark.Domain.Veiculos;

public sealed class Veiculo : AuditableEntity
{
    private Veiculo()
    {
    }

    public Guid UsuarioId { get; private set; }

    public Placa Placa { get; private set; } = null!;

    public string Modelo { get; private set; } = null!;

    /// <summary>
    /// Veículos removidos são apenas desativados: o histórico de reservas continua apontando para eles.
    /// </summary>
    public bool Ativo { get; private set; }

    public static Veiculo Criar(Guid usuarioId, Placa placa, string modelo) => new()
    {
        UsuarioId = usuarioId,
        Placa = placa,
        Modelo = modelo.Trim(),
        Ativo = true
    };

    public bool PertenceA(Guid usuarioId) => UsuarioId == usuarioId;

    public void Atualizar(Placa placa, string modelo)
    {
        Placa = placa;
        Modelo = modelo.Trim();
    }

    public void Remover() => Ativo = false;
}
