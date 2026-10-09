using Mediator;
using SmartPark.Contracts.Veiculos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Veiculos.AtualizarVeiculo;

public sealed record AtualizarVeiculoCommand(Guid VeiculoId, string Placa, string Modelo) : ICommand<Result<VeiculoResponse>>;
