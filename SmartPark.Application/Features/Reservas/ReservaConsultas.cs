using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Reservas;
using SmartPark.Domain.Pagamentos;
using SmartPark.Domain.Reservas;
using SmartPark.Domain.Veiculos;

namespace SmartPark.Application.Features.Reservas;

/// <summary>Leitura de reservas com os dados de exibição (estacionamento, placa e pagamento).</summary>
internal static class ReservaConsultas
{
    /// <summary>
    /// Projeção por inicializador de membros (e não por construtor), para que filtros e ordenações
    /// aplicados depois continuem sendo traduzidos para SQL.
    /// </summary>
    public static IQueryable<ReservaLinha> Linhas(this IApplicationDbContext db) =>
        from r in db.Reservas
        join e in db.Estacionamentos on r.EstacionamentoId equals e.Id
        join v in db.Veiculos on r.VeiculoId equals v.Id
        join p in db.Pagamentos on r.Id equals p.ReservaId into pagamentos
        from p in pagamentos.DefaultIfEmpty()
        select new ReservaLinha
        {
            Id = r.Id,
            UsuarioId = r.UsuarioId,
            EstacionamentoId = r.EstacionamentoId,
            EstacionamentoNome = e.Nome,
            VeiculoId = r.VeiculoId,
            Placa = v.Placa,
            Inicio = r.Inicio,
            Fim = r.Fim,
            ValorEstimado = r.ValorEstimado,
            Status = r.Status,
            StatusPagamento = p != null ? p.Status : (StatusPagamento?)null,
            CriadoEm = r.CriadoEm
        };

    public static ReservaResponse ParaResponse(this ReservaLinha linha) =>
        new(
            linha.Id,
            linha.EstacionamentoId,
            linha.EstacionamentoNome,
            linha.VeiculoId,
            linha.Placa.Valor,
            linha.Inicio,
            linha.Fim,
            linha.ValorEstimado,
            linha.Status.ToString(),
            linha.StatusPagamento?.ToString(),
            linha.CriadoEm);
}

internal sealed class ReservaLinha
{
    public required Guid Id { get; init; }

    public required Guid UsuarioId { get; init; }

    public required Guid EstacionamentoId { get; init; }

    public required string EstacionamentoNome { get; init; }

    public required Guid VeiculoId { get; init; }

    public required Placa Placa { get; init; }

    public required DateTimeOffset Inicio { get; init; }

    public required DateTimeOffset Fim { get; init; }

    public required decimal ValorEstimado { get; init; }

    public required StatusReserva Status { get; init; }

    public required StatusPagamento? StatusPagamento { get; init; }

    public required DateTimeOffset CriadoEm { get; init; }
}
