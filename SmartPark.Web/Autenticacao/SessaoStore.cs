using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartPark.Contracts.Autenticacao;

namespace SmartPark.Web.Autenticacao;

/// <summary>
/// Sessão do usuário no circuito atual. O token fica cifrado no localStorage do navegador
/// (ProtectedLocalStorage) e é lido uma vez por circuito.
/// </summary>
public sealed class SessaoStore(ProtectedLocalStorage storage, TimeProvider timeProvider)
{
    private const string Chave = "smartpark.sessao";

    private TokenResponse? _sessao;
    private bool _carregada;

    /// <summary>Disparado quando a sessão termina (logout ou token rejeitado pela API).</summary>
    public event Action? SessaoEncerrada;

    public async Task<TokenResponse?> ObterAsync()
    {
        if (!_carregada)
        {
            try
            {
                var lida = await storage.GetAsync<TokenResponse>(Chave);
                _sessao = lida.Success ? lida.Value : null;
            }
            catch (CryptographicException)
            {
                // Chaves de proteção trocadas (ex.: reinício em desenvolvimento): a sessão antiga não serve mais.
                await storage.DeleteAsync(Chave);
                _sessao = null;
            }

            _carregada = true;
        }

        if (_sessao is not null && _sessao.ExpiraEm <= timeProvider.GetUtcNow())
        {
            await EncerrarAsync();
        }

        return _sessao;
    }

    public async Task SalvarAsync(TokenResponse sessao)
    {
        _sessao = sessao;
        _carregada = true;
        await storage.SetAsync(Chave, sessao);
    }

    public async Task EncerrarAsync()
    {
        var havia = _sessao is not null;
        _sessao = null;
        _carregada = true;
        await storage.DeleteAsync(Chave);

        if (havia)
        {
            SessaoEncerrada?.Invoke();
        }
    }
}
