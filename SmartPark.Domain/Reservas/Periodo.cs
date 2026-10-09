namespace SmartPark.Domain.Reservas;

/// <summary>Intervalo semiaberto [Inicio, Fim): uma reserva que termina às 10h não colide com outra que começa às 10h.</summary>
public readonly record struct Periodo(DateTimeOffset Inicio, DateTimeOffset Fim)
{
    public TimeSpan Duracao => Fim - Inicio;

    public bool SobrepoeA(Periodo outro) => Inicio < outro.Fim && outro.Inicio < Fim;
}
