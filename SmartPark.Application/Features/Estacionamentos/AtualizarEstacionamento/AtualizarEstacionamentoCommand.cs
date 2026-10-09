using Mediator;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Estacionamentos.AtualizarEstacionamento;

public sealed record AtualizarEstacionamentoCommand(
    Guid EstacionamentoId,
    string Nome,
    string Endereco,
    double? Latitude,
    double? Longitude,
    int CapacidadeTotal,
    int CapacidadeReservavel,
    decimal ValorHora) : ICommand<Result<EstacionamentoResponse>>, IDadosEstacionamento;
