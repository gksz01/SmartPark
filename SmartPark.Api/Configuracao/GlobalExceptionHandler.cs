using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmartPark.Api.Configuracao;

/// <summary>
/// Último recurso do pipeline. Erros de validação da entrada (lançados pelo ValidationBehavior) viram 400
/// com os campos; qualquer outra exceção vira 500 sem detalhes internos.
/// </summary>
internal sealed partial class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problema = exception switch
        {
            ValidationException validacao => new HttpValidationProblemDetails(
                validacao.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Dados inválidos",
                Detail = string.Join(" ", validacao.Errors.Select(e => e.ErrorMessage).Distinct())
            },
            BadHttpRequestException requisicaoInvalida => new ProblemDetails
            {
                Status = requisicaoInvalida.StatusCode,
                Title = "Requisição inválida",
                Detail = "O corpo ou os parâmetros da requisição não estão no formato esperado."
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro inesperado",
                Detail = "Ocorreu um erro inesperado. Tente novamente em instantes."
            }
        };

        if (problema.Status == StatusCodes.Status500InternalServerError)
        {
            LogErroInesperado(logger, exception);
        }

        httpContext.Response.StatusCode = problema.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problema
        });
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Erro não tratado")]
    private static partial void LogErroInesperado(ILogger logger, Exception exception);
}
