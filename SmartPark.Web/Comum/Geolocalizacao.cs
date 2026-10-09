using Microsoft.JSInterop;

namespace SmartPark.Web.Comum;

/// <summary>Posição atual do navegador, usada para ordenar estacionamentos por distância (RF04).</summary>
public sealed class Geolocalizacao(IJSRuntime js) : IAsyncDisposable
{
    private IJSObjectReference? _modulo;

    public async Task<Posicao?> ObterAsync()
    {
        _modulo ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/geolocalizacao.js");
        return await _modulo.InvokeAsync<Posicao?>("obterPosicao");
    }

    public async ValueTask DisposeAsync()
    {
        if (_modulo is not null)
        {
            try
            {
                await _modulo.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuito já encerrado: não há o que liberar no navegador.
            }
        }
    }
}

public sealed record Posicao(double Latitude, double Longitude);
