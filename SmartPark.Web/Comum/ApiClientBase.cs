using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SmartPark.Web.Autenticacao;

namespace SmartPark.Web.Comum;

/// <summary>
/// Base dos clientes HTTP de cada fatia. Anexa o token da sessão em cada chamada — lido do
/// <see cref="SessaoStore"/> do circuito, por isso não é um DelegatingHandler, cujo escopo de DI é outro —
/// e converte ProblemDetails da API em mensagens para a interface.
/// </summary>
public abstract class ApiClientBase(HttpClient http, SessaoStore sessao)
{
    private const string FalhaDeConexao = "Não foi possível falar com o servidor. Verifique sua conexão e tente novamente.";

    protected Task<ApiResult<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default) =>
        EnviarAsync<T>(HttpMethod.Get, url, null, cancellationToken);

    protected Task<ApiResult<T>> PostAsync<T>(string url, object corpo, CancellationToken cancellationToken = default) =>
        EnviarAsync<T>(HttpMethod.Post, url, corpo, cancellationToken);

    protected Task<ApiResult<T>> PutAsync<T>(string url, object corpo, CancellationToken cancellationToken = default) =>
        EnviarAsync<T>(HttpMethod.Put, url, corpo, cancellationToken);

    protected async Task<ApiResult> EnviarSemRetornoAsync(HttpMethod metodo, string url, object? corpo = null, CancellationToken cancellationToken = default)
    {
        try
        {
            using var resposta = await ExecutarAsync(metodo, url, corpo, cancellationToken);
            return resposta.IsSuccessStatusCode
                ? ApiResult.Ok()
                : ApiResult.Falha(await LerErroAsync(resposta, cancellationToken), (int)resposta.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult.Falha(FalhaDeConexao, 0);
        }
    }

    private async Task<ApiResult<T>> EnviarAsync<T>(HttpMethod metodo, string url, object? corpo, CancellationToken cancellationToken)
    {
        try
        {
            using var resposta = await ExecutarAsync(metodo, url, corpo, cancellationToken);
            if (!resposta.IsSuccessStatusCode)
            {
                return ApiResult<T>.Falha(await LerErroAsync(resposta, cancellationToken), (int)resposta.StatusCode);
            }

            var valor = await resposta.Content.ReadFromJsonAsync<T>(cancellationToken);
            return valor is null
                ? ApiResult<T>.Falha("A resposta do servidor veio vazia.", (int)resposta.StatusCode)
                : ApiResult<T>.Ok(valor);
        }
        catch (HttpRequestException)
        {
            return ApiResult<T>.Falha(FalhaDeConexao, 0);
        }
    }

    private async Task<HttpResponseMessage> ExecutarAsync(HttpMethod metodo, string url, object? corpo, CancellationToken cancellationToken)
    {
        using var requisicao = new HttpRequestMessage(metodo, url);
        if (corpo is not null)
        {
            requisicao.Content = JsonContent.Create(corpo, corpo.GetType());
        }

        var token = await sessao.ObterAsync();
        if (token is not null)
        {
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        var resposta = await http.SendAsync(requisicao, cancellationToken);

        // Token vencido ou revogado: encerra a sessão, e o layout leva o usuário de volta ao login.
        if (resposta.StatusCode == HttpStatusCode.Unauthorized && token is not null)
        {
            await sessao.EncerrarAsync();
        }

        return resposta;
    }

    private static async Task<string> LerErroAsync(HttpResponseMessage resposta, CancellationToken cancellationToken)
    {
        try
        {
            var problema = await resposta.Content.ReadFromJsonAsync<ProblemaApi>(cancellationToken);
            if (problema?.Errors is { Count: > 0 } erros)
            {
                return string.Join(" ", erros.Values.SelectMany(m => m).Distinct());
            }

            if (!string.IsNullOrWhiteSpace(problema?.Detail))
            {
                return problema.Detail;
            }
        }
        catch (System.Text.Json.JsonException)
        {
            // Corpo que não é ProblemDetails: cai na mensagem padrão do status.
        }

        return resposta.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "Sua sessão expirou. Entre novamente.",
            HttpStatusCode.Forbidden => "Você não tem permissão para esta operação.",
            HttpStatusCode.NotFound => "Registro não encontrado.",
            HttpStatusCode.ServiceUnavailable => "Serviço temporariamente indisponível. Tente novamente em instantes.",
            _ => "Não foi possível concluir a operação."
        };
    }

    private sealed record ProblemaApi(string? Title, string? Detail, int? Status, Dictionary<string, string[]>? Errors);
}
