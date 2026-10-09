using System.Globalization;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Web.Autenticacao;
using SmartPark.Web.Comum;

namespace SmartPark.Web.Features.Estacionamentos;

public sealed class EstacionamentosApi(HttpClient http, SessaoStore sessao) : ApiClientBase(http, sessao)
{
    public Task<ApiResult<List<EstacionamentoResponse>>> ListarAsync(string? busca, Posicao? posicao)
    {
        var parametros = new List<string>();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            parametros.Add($"busca={Uri.EscapeDataString(busca)}");
        }

        if (posicao is not null)
        {
            parametros.Add($"latitude={posicao.Latitude.ToString(CultureInfo.InvariantCulture)}");
            parametros.Add($"longitude={posicao.Longitude.ToString(CultureInfo.InvariantCulture)}");
        }

        var query = parametros.Count == 0 ? string.Empty : "?" + string.Join("&", parametros);
        return GetAsync<List<EstacionamentoResponse>>($"/api/estacionamentos{query}");
    }

    public Task<ApiResult<EstacionamentoResponse>> ObterAsync(Guid estacionamentoId) =>
        GetAsync<EstacionamentoResponse>($"/api/estacionamentos/{estacionamentoId}");

    public Task<ApiResult<OcupacaoResponse>> OcupacaoAsync(Guid estacionamentoId) =>
        GetAsync<OcupacaoResponse>($"/api/estacionamentos/{estacionamentoId}/ocupacao");

    public Task<ApiResult<DisponibilidadeResponse>> DisponibilidadeAsync(Guid estacionamentoId, DateTimeOffset inicio, DateTimeOffset fim) =>
        GetAsync<DisponibilidadeResponse>(
            $"/api/estacionamentos/{estacionamentoId}/disponibilidade?inicio={Instante(inicio)}&fim={Instante(fim)}");

    private static string Instante(DateTimeOffset valor) =>
        Uri.EscapeDataString(valor.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
}
