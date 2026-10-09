namespace SmartPark.Contracts.Vagas;

public sealed record CriarVagasEmLoteRequest(string Prefixo, int Quantidade);

/// <param name="Status">Livre, Ocupada ou Indisponivel.</param>
public sealed record AtualizarStatusVagaRequest(string Status);

public sealed record VagaResponse(Guid Id, string Identificador, string Status);
