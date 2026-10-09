using Mediator;
using Microsoft.EntityFrameworkCore;
using SmartPark.Application.Abstractions.Autenticacao;
using SmartPark.Application.Abstractions.Data;
using SmartPark.Contracts.Estacionamentos;
using SmartPark.Domain.Common;

namespace SmartPark.Application.Features.Estacionamentos.ListarEstacionamentos;

public sealed class ListarEstacionamentosHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IQueryHandler<ListarEstacionamentosQuery, Result<IReadOnlyList<EstacionamentoResponse>>>
{
    public async ValueTask<Result<IReadOnlyList<EstacionamentoResponse>>> Handle(ListarEstacionamentosQuery query, CancellationToken cancellationToken)
    {
        var estacionamentos = db.Estacionamentos.AsNoTracking();

        if (!(query.IncluirInativos && currentUser.EhAdministrador))
        {
            estacionamentos = estacionamentos.Where(e => e.Ativo);
        }

        if (!string.IsNullOrWhiteSpace(query.Busca))
        {
            var busca = query.Busca.Trim().ToLower();
            estacionamentos = estacionamentos.Where(e => e.Nome.ToLower().Contains(busca) || e.Endereco.ToLower().Contains(busca));
        }

        var linhas = await estacionamentos
            .Select(e => new { Estacionamento = e, TotalVagas = e.Vagas.Count() })
            .ToListAsync(cancellationToken);

        var respostas = linhas.Select(l =>
        {
            double? distancia = query is { Latitude: { } lat, Longitude: { } lng }
                && l.Estacionamento is { Latitude: { } eLat, Longitude: { } eLng }
                    ? Math.Round(Distancia.EmKm(lat, lng, eLat, eLng), 2)
                    : null;

            return l.Estacionamento.ParaResponse(l.TotalVagas, distancia);
        });

        // Com posição, os mais próximos primeiro (sem coordenadas vão para o fim); sem posição, por nome.
        var ordenadas = query.Latitude.HasValue
            ? respostas.OrderBy(r => r.DistanciaKm ?? double.MaxValue).ThenBy(r => r.Nome)
            : respostas.OrderBy(r => r.Nome);

        return Result.Success<IReadOnlyList<EstacionamentoResponse>>([.. ordenadas]);
    }
}
