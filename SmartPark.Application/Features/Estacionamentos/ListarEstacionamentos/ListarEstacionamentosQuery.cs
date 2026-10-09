using Mediator;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Estacionamentos.ListarEstacionamentos;

/// <param name="Busca">Trecho do nome ou do endereço.</param>
/// <param name="Latitude">Posição do motorista; com a longitude, ordena o resultado por distância.</param>
/// <param name="Longitude">Posição do motorista.</param>
/// <param name="IncluirInativos">Considerado apenas para administradores.</param>
public sealed record ListarEstacionamentosQuery(string? Busca, double? Latitude, double? Longitude, bool IncluirInativos)
    : IQuery<Result<IReadOnlyList<EstacionamentoResponse>>>;
