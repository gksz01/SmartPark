using SmartPark.Domain.Common;

namespace SmartPark.Api.Configuracao;

/// <summary>Tradução de <see cref="Result"/> para HTTP: falhas viram ProblemDetails com o código do erro.</summary>
public static class ResultExtensions
{
    public static IResult ToOk<T>(this Result<T> result) =>
        result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToProblem();

    public static IResult ToCreated<T>(this Result<T> result, Func<T, string> location) =>
        result.IsSuccess ? Results.Created(location(result.Value), result.Value) : result.Error.ToProblem();

    public static IResult ToNoContent(this Result result) =>
        result.IsSuccess ? Results.NoContent() : result.Error.ToProblem();

    public static IResult ToProblem(this Error error)
    {
        var (status, titulo) = error.Type switch
        {
            ErrorType.Validation => (StatusCodes.Status400BadRequest, "Dados inválidos"),
            ErrorType.NotFound => (StatusCodes.Status404NotFound, "Não encontrado"),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflito"),
            ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "Não autenticado"),
            ErrorType.Forbidden => (StatusCodes.Status403Forbidden, "Acesso negado"),
            ErrorType.Unavailable => (StatusCodes.Status503ServiceUnavailable, "Serviço indisponível"),
            _ => (StatusCodes.Status400BadRequest, "Operação não realizada")
        };

        return Results.Problem(
            statusCode: status,
            title: titulo,
            detail: error.Message,
            extensions: new Dictionary<string, object?> { ["code"] = error.Code });
    }
}
