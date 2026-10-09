using SmartPark.Domain.Common;
using SmartPark.Domain.Estacionamentos;
using SmartPark.Domain.Veiculos;

namespace SmartPark.Domain.Reservas;

/// <summary>
/// Reserva de capacidade de um estacionamento por um período. Não aponta para nenhuma vaga física:
/// ao chegar, o motorista estaciona em qualquer vaga livre ou é direcionado por um funcionário.
/// </summary>
public sealed class Reserva : AuditableEntity
{
    public static readonly TimeSpan DuracaoMinima = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan DuracaoMaxima = TimeSpan.FromHours(24);
    public static readonly TimeSpan AntecedenciaMaxima = TimeSpan.FromDays(30);

    /// <summary>Estados em que a reserva consome capacidade do estacionamento.</summary>
    public static readonly StatusReserva[] StatusQueOcupamCapacidade = [StatusReserva.Confirmada, StatusReserva.EmUso];

    private Reserva()
    {
    }

    public Guid UsuarioId { get; private set; }

    public Guid VeiculoId { get; private set; }

    public Guid EstacionamentoId { get; private set; }

    public DateTimeOffset Inicio { get; private set; }

    public DateTimeOffset Fim { get; private set; }

    public decimal ValorEstimado { get; private set; }

    public StatusReserva Status { get; private set; }

    public DateTimeOffset? CanceladaEm { get; private set; }

    public Periodo Periodo => new(Inicio, Fim);

    /// <summary>Regras de período, compartilhadas com a consulta de disponibilidade.</summary>
    public static Result ValidarPeriodo(Periodo periodo, DateTimeOffset agora)
    {
        if (periodo.Fim <= periodo.Inicio)
        {
            return ReservaErrors.FimAntesDoInicio;
        }

        if (periodo.Inicio <= agora)
        {
            return ReservaErrors.InicioNoPassado;
        }

        if (periodo.Duracao < DuracaoMinima || periodo.Duracao > DuracaoMaxima)
        {
            return ReservaErrors.DuracaoForaDoLimite;
        }

        return periodo.Inicio > agora + AntecedenciaMaxima
            ? ReservaErrors.AntecedenciaExcedida
            : Result.Success();
    }

    /// <param name="reservasAtivas">Períodos das reservas que já ocupam capacidade e se sobrepõem ao período pedido.</param>
    public static Result<Reserva> Criar(
        Guid usuarioId,
        Veiculo veiculo,
        Estacionamento estacionamento,
        Periodo periodo,
        IEnumerable<Periodo> reservasAtivas,
        DateTimeOffset agora)
    {
        if (!veiculo.Ativo || !veiculo.PertenceA(usuarioId))
        {
            return ReservaErrors.VeiculoInvalido;
        }

        var periodoValido = ValidarPeriodo(periodo, agora);
        if (periodoValido.IsFailure)
        {
            return periodoValido.Error;
        }

        var capacidade = estacionamento.GarantirCapacidade(periodo, reservasAtivas);
        if (capacidade.IsFailure)
        {
            return capacidade.Error;
        }

        return new Reserva
        {
            UsuarioId = usuarioId,
            VeiculoId = veiculo.Id,
            EstacionamentoId = estacionamento.Id,
            Inicio = periodo.Inicio,
            Fim = periodo.Fim,
            ValorEstimado = estacionamento.CalcularValor(periodo),
            Status = StatusReserva.Confirmada
        };
    }

    public bool PertenceA(Guid usuarioId) => UsuarioId == usuarioId;

    public Result Cancelar(DateTimeOffset agora)
    {
        if (Status != StatusReserva.Confirmada || agora >= Inicio)
        {
            return ReservaErrors.NaoCancelavel;
        }

        Status = StatusReserva.Cancelada;
        CanceladaEm = agora;
        return Result.Success();
    }
}
