using Mediator;
using SmartPark.Contracts.Veiculos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Veiculos.CadastrarVeiculo;

public sealed record CadastrarVeiculoCommand(string Placa, string Modelo) : ICommand<Result<VeiculoResponse>>;
