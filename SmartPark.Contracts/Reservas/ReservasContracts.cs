namespace SmartPark.Contracts.Reservas;

public sealed record CriarReservaRequest(Guid EstacionamentoId, Guid VeiculoId, DateTimeOffset Inicio, DateTimeOffset Fim);

public sealed record ReservaResponse(
    Guid Id,
    Guid EstacionamentoId,
    string EstacionamentoNome,
    Guid VeiculoId,
    string Placa,
    DateTimeOffset Inicio,
    DateTimeOffset Fim,
    decimal ValorEstimado,
    string Status,
    string? StatusPagamento,
    DateTimeOffset CriadaEm);
