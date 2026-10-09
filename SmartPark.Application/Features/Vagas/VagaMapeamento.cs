using SmartPark.Contracts.Vagas;
using SmartPark.Domain.Estacionamentos;

namespace SmartPark.Application.Features.Vagas;

internal static class VagaMapeamento
{
    public static VagaResponse ParaResponse(this Vaga vaga) =>
        new(vaga.Id, vaga.Identificador, vaga.Status.ToString());
}
