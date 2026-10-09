using SmartPark.Domain.Common;

namespace SmartPark.Domain.Estacionamentos;

/// <summary>
/// Vaga física. Faz parte do agregado <see cref="Estacionamento"/>: só ele cria, altera e remove vagas.
/// A reserva nunca aponta para uma vaga — ela reserva capacidade do estacionamento.
/// </summary>
public sealed class Vaga : AuditableEntity
{
    private Vaga()
    {
    }

    public Guid EstacionamentoId { get; private set; }

    public string Identificador { get; private set; } = null!;

    public StatusVaga Status { get; private set; }

    internal static Vaga Criar(Guid estacionamentoId, string identificador) => new()
    {
        EstacionamentoId = estacionamentoId,
        Identificador = identificador,
        Status = StatusVaga.Livre
    };

    internal void AlterarStatus(StatusVaga status) => Status = status;
}
