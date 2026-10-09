namespace SmartPark.Contracts.Pagamentos;

/// <param name="Metodo">Pix ou Cartao.</param>
/// <param name="NumeroCartao">Obrigatório para cartão. Só é repassado ao gateway; o SmartPark não o armazena.</param>
public sealed record PagarReservaRequest(string Metodo, string? NumeroCartao);

public sealed record PagamentoResponse(
    Guid Id,
    Guid ReservaId,
    decimal Valor,
    string Metodo,
    string Status,
    DateTimeOffset? ProcessadoEm,
    string? TransacaoExternaId,
    string? MotivoRecusa);
