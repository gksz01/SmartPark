using System.Globalization;

namespace SmartPark.Web.Comum;

public static class Formatos
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public static string Moeda(decimal valor) => valor.ToString("C", PtBr);

    public static string Km(double km) => $"{km.ToString("0.0", PtBr)} km";
}
