using Mediator;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Veiculos.RemoverVeiculo;

public sealed record RemoverVeiculoCommand(Guid VeiculoId) : ICommand<Result>;
