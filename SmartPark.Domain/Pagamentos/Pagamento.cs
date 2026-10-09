using SmartPark.Domain.Common;
using SmartPark.Domain.Reservas;

namespace SmartPark.Domain.Pagamentos;

/// <summary>
/// Pagamento de uma reserva. O processamento é delegado a um gateway externo; aqui fica só o estado
/// que o SmartPark precisa para liberar a saída e para estornar em caso de cancelamento.
/// </summary>
public sealed class Pagamento : AuditableEntity
{
    private Pagamento()
    {
    }

    public Guid ReservaId { get; private set; }

    public decimal Valor { get; private set; }

    public MetodoPagamento Metodo { get; private set; }

    public StatusPagamento Status { get; private set; }

    public DateTimeOffset? ProcessadoEm { get; private set; }

    public string? TransacaoExternaId { get; private set; }

    public string? MotivoRecusa { get; private set; }

    public static Result<Pagamento> Iniciar(Reserva reserva, MetodoPagamento metodo)
    {
        if (reserva.Status != StatusReserva.Confirmada)
        {
            return ReservaErrors.NaoPagavel;
        }

        return new Pagamento
        {
            ReservaId = reserva.Id,
            Valor = reserva.ValorEstimado,
            Metodo = metodo,
            Status = StatusPagamento.Pendente
        };
    }

    /// <summary>Uma tentativa recusada ou interrompida pode ser refeita, inclusive com outro método.</summary>
    public Result NovaTentativa(Reserva reserva, MetodoPagamento metodo)
    {
        if (Status == StatusPagamento.Aprovado)
        {
            return PagamentoErrors.JaAprovado;
        }

        if (reserva.Status != StatusReserva.Confirmada || Status == StatusPagamento.Estornado)
        {
            return ReservaErrors.NaoPagavel;
        }

        Valor = reserva.ValorEstimado;
        Metodo = metodo;
        Status = StatusPagamento.Pendente;
        MotivoRecusa = null;
        return Result.Success();
    }

    public void Aprovar(string transacaoExternaId, DateTimeOffset em)
    {
        Status = StatusPagamento.Aprovado;
        TransacaoExternaId = transacaoExternaId;
        MotivoRecusa = null;
        ProcessadoEm = em;
    }

    public void Recusar(string motivo, DateTimeOffset em)
    {
        Status = StatusPagamento.Recusado;
        MotivoRecusa = motivo;
        ProcessadoEm = em;
    }

    public Result Estornar(DateTimeOffset em)
    {
        if (Status != StatusPagamento.Aprovado)
        {
            return PagamentoErrors.NaoEstornavel;
        }

        Status = StatusPagamento.Estornado;
        ProcessadoEm = em;
        return Result.Success();
    }
}
