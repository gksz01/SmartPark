namespace SmartPark.Application.Common;

/// <summary>Os contratos HTTP trafegam enums como texto; aqui fica a conversão aceita pela API.</summary>
internal static class EnumTexto
{
    /// <summary>Aceita só os nomes do enum (sem diferenciar maiúsculas), nunca o valor numérico.</summary>
    public static bool EhValido<TEnum>(string? valor)
        where TEnum : struct, Enum =>
        valor is not null && Enum.GetNames<TEnum>().Contains(valor, StringComparer.OrdinalIgnoreCase);

    public static TEnum Converter<TEnum>(string valor)
        where TEnum : struct, Enum =>
        Enum.Parse<TEnum>(valor, ignoreCase: true);

    public static string Opcoes<TEnum>()
        where TEnum : struct, Enum =>
        string.Join(", ", Enum.GetNames<TEnum>());
}
