namespace SmartPark.Domain.Reservas;

public enum StatusReserva
{
    Confirmada = 1,
    Cancelada = 2,

    // Os três estados abaixo passam a ser usados na fase 2, com o controle de entrada e saída.
    EmUso = 3,
    Concluida = 4,
    Expirada = 5
}
