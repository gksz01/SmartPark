using Mediator;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Autenticacao.Login;

/// <summary>É comando, não consulta: altera o contador de tentativas e o bloqueio da conta.</summary>
public sealed record LoginCommand(string Email, string Senha) : ICommand<Result<TokenResponse>>;
