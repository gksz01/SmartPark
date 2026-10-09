using Mediator;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Disponibilidade.ObterOcupacao;

/// <summary>Ocupação física atual das vagas (RF06).</summary>
public sealed record ObterOcupacaoQuery(Guid EstacionamentoId) : IQuery<Result<OcupacaoResponse>>;
