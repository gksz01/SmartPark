using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Behaviors;

public sealed partial class LoggingBehavior<TMessage, TResponse>(ILogger<LoggingBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var nome = typeof(TMessage).Name;
        var inicio = Stopwatch.GetTimestamp();

        var resposta = await next(message, cancellationToken);

        var duracao = Stopwatch.GetElapsedTime(inicio).TotalMilliseconds;
        if (resposta is Result { IsFailure: true } falha)
        {
            LogFalha(logger, nome, falha.Error.Code, duracao);
        }
        else
        {
            LogSucesso(logger, nome, duracao);
        }

        return resposta;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "{Mensagem} concluída em {DuracaoMs:0.0} ms")]
    private static partial void LogSucesso(ILogger logger, string mensagem, double duracaoMs);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Mensagem} falhou com {Codigo} em {DuracaoMs:0.0} ms")]
    private static partial void LogFalha(ILogger logger, string mensagem, string codigo, double duracaoMs);
}
