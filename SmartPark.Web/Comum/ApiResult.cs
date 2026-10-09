namespace SmartPark.Web.Comum;

/// <summary>Resposta de uma chamada à API já traduzida para a interface: sucesso ou mensagem pronta para exibir.</summary>
public class ApiResult
{
    protected ApiResult(bool sucesso, string? erro, int status)
    {
        Sucesso = sucesso;
        Erro = erro;
        Status = status;
    }

    public bool Sucesso { get; }

    public string? Erro { get; }

    public int Status { get; }

    public static ApiResult Ok() => new(true, null, 200);

    public static ApiResult Falha(string erro, int status) => new(false, erro, status);
}

public sealed class ApiResult<T> : ApiResult
{
    private ApiResult(bool sucesso, T? valor, string? erro, int status)
        : base(sucesso, erro, status) => Valor = valor;

    public T? Valor { get; }

    public static ApiResult<T> Ok(T valor) => new(true, valor, null, 200);

    public static new ApiResult<T> Falha(string erro, int status) => new(false, default, erro, status);
}
