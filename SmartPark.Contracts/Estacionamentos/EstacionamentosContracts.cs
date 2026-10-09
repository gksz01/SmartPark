namespace SmartPark.Contracts.Estacionamentos;

public sealed record EstacionamentoRequest(
    string Nome,
    string Endereco,
    double? Latitude,
    double? Longitude,
    int CapacidadeTotal,
    int CapacidadeReservavel,
    decimal ValorHora);

public sealed record AlterarSituacaoEstacionamentoRequest(bool Ativo);

public sealed record EstacionamentoResponse(
    Guid Id,
    string Nome,
    string Endereco,
    double? Latitude,
    double? Longitude,
    int CapacidadeTotal,
    int CapacidadeReservavel,
    decimal ValorHora,
    bool Ativo,
    int TotalVagas,
    double? DistanciaKm);

public sealed record OcupacaoResponse(
    Guid EstacionamentoId,
    int TotalVagas,
    int Livres,
    int Ocupadas,
    int Indisponiveis);

public sealed record DisponibilidadeResponse(
    Guid EstacionamentoId,
    DateTimeOffset Inicio,
    DateTimeOffset Fim,
    int CapacidadeReservavel,
    int Reservadas,
    int Disponivel,
    decimal ValorEstimado);
