using Mediator;
using SmartPark.Contracts.Autenticacao;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Autenticacao.ObterUsuarioAtual;

public sealed record ObterUsuarioAtualQuery : IQuery<Result<UsuarioResponse>>;
