using SmartPark.Domain.Reservas;

namespace SmartPark.Application.Common;

internal static class ConsultasDeReserva
{
    /// <summary>
    /// Reservas que consomem capacidade (espelha <see cref="Reserva.StatusQueOcupamCapacidade"/>). Escrito
    /// como comparações simples para traduzir igual em PostgreSQL e SQL Server.
    /// </summary>
    public static IQueryable<Reserva> QueOcupamCapacidade(this IQueryable<Reserva> reservas) =>
        reservas.Where(r => r.Status == StatusReserva.Confirmada || r.Status == StatusReserva.EmUso);

    public static IQueryable<Reserva> QueSobrepoem(this IQueryable<Reserva> reservas, Periodo periodo) =>
        reservas.Where(r => r.Inicio < periodo.Fim && r.Fim > periodo.Inicio);
}
