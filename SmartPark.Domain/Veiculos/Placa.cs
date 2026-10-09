using System.Text.RegularExpressions;
using SmartPark.Domain.Common;

namespace SmartPark.Domain.Veiculos;

/// <summary>
/// Placa brasileira normalizada (maiúsculas, sem hífen ou espaços). Aceita o padrão antigo
/// (ABC1234) e o Mercosul (ABC1D23) — os dois cabem em LLL N [L|N] NN.
/// </summary>
public sealed partial record Placa
{
    private Placa(string valor) => Valor = valor;

    public string Valor { get; }

    public static Result<Placa> Criar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return VeiculoErrors.PlacaObrigatoria;
        }

        var normalizada = Normalizar(valor);

        return PadraoPlaca().IsMatch(normalizada)
            ? new Placa(normalizada)
            : VeiculoErrors.PlacaInvalida;
    }

    public static string Normalizar(string valor) =>
        valor.Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Trim()
            .ToUpperInvariant();

    public override string ToString() => Valor;

    [GeneratedRegex("^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$")]
    private static partial Regex PadraoPlaca();
}
