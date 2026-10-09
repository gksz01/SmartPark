namespace SmartPark.Contracts.Veiculos;

public sealed record VeiculoRequest(string Placa, string Modelo);

public sealed record VeiculoResponse(Guid Id, string Placa, string Modelo);
