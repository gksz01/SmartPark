using System.Globalization;

namespace SmartPark.Web.Comum;

/// <summary>
/// Converte entre o horário de parede que o usuário escolhe na tela e o instante (UTC) que a API usa.
/// O fuso dos estacionamentos é configurável em <c>Web:FusoHorario</c> (padrão: Brasília).
/// </summary>
public sealed class FusoHorario(IConfiguration configuration)
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    private readonly TimeZoneInfo _fuso = TimeZoneInfo.FindSystemTimeZoneById(
        configuration["Web:FusoHorario"] ?? "America/Sao_Paulo");

    public DateTime Agora => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _fuso).DateTime;

    public DateTimeOffset ParaInstante(DateTime horarioLocal)
    {
        var semTipo = DateTime.SpecifyKind(horarioLocal, DateTimeKind.Unspecified);
        return new DateTimeOffset(semTipo, _fuso.GetUtcOffset(semTipo)).ToUniversalTime();
    }

    public DateTime ParaLocal(DateTimeOffset instante) => TimeZoneInfo.ConvertTime(instante, _fuso).DateTime;

    public string Formatar(DateTimeOffset instante) => ParaLocal(instante).ToString("dd/MM/yyyy HH:mm", PtBr);
}
