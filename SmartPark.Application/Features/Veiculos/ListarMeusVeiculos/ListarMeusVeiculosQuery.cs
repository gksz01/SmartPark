using Mediator;
using SmartPark.Contracts.Veiculos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Veiculos.ListarMeusVeiculos;

public sealed record ListarMeusVeiculosQuery : IQuery<Result<IReadOnlyList<VeiculoResponse>>>;
