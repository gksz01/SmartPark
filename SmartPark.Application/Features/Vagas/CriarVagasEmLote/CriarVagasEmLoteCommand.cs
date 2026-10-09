using Mediator;
using SmartPark.Contracts.Vagas;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Vagas.CriarVagasEmLote;

/// <summary>Cria vagas numeradas em sequência a partir do prefixo (ex.: A01, A02...).</summary>
public sealed record CriarVagasEmLoteCommand(Guid EstacionamentoId, string Prefixo, int Quantidade)
    : ICommand<Result<IReadOnlyList<VagaResponse>>>;
