using SmartPark.Domain.Reservas;

namespace SmartPark.Domain.Estacionamentos;

public static class CalculadoraOcupacao
{
    /// <summary>
    /// Maior número de reservas simultâneas dentro da janela. Contar todas as reservas que tocam a
    /// janela superestimaria a ocupação: duas reservas de 8h–9h e 10h–11h nunca coexistem.
    /// </summary>
    public static int PicoSimultaneo(IEnumerable<Periodo> reservas, Periodo janela)
    {
        var eventos = new List<(DateTimeOffset Instante, int Delta)>();

        foreach (var reserva in reservas.Where(r => r.SobrepoeA(janela)))
        {
            eventos.Add((reserva.Inicio > janela.Inicio ? reserva.Inicio : janela.Inicio, +1));
            eventos.Add((reserva.Fim < janela.Fim ? reserva.Fim : janela.Fim, -1));
        }

        // No mesmo instante, a saída vem antes da entrada (intervalos semiabertos).
        eventos.Sort((a, b) => a.Instante != b.Instante
            ? a.Instante.CompareTo(b.Instante)
            : a.Delta.CompareTo(b.Delta));

        int atual = 0, pico = 0;
        foreach (var (_, delta) in eventos)
        {
            atual += delta;
            pico = Math.Max(pico, atual);
        }

        return pico;
    }
}
