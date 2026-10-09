using FluentValidation;
using Mediator;

namespace SmartPark.Application.Behaviors;

/// <summary>
/// Executa os validators FluentValidation da mensagem antes do handler. Erros de formato da entrada
/// interrompem o pipeline com <see cref="ValidationException"/>, que o host converte em 400.
/// </summary>
public sealed class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TMessage>(message);
        var falhas = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var validator in validators)
        {
            var resultado = await validator.ValidateAsync(context, cancellationToken);
            falhas.AddRange(resultado.Errors);
        }

        if (falhas.Count != 0)
        {
            throw new ValidationException(falhas);
        }

        return await next(message, cancellationToken);
    }
}
