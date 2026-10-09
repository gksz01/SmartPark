using Mediator;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Autenticacao.RegistrarMotorista;

public sealed record RegistrarMotoristaCommand(string Nome, string Email, string Senha)
    : ICommand<Result<TokenResponse>>;
