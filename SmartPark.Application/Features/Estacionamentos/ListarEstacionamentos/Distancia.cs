namespace SmartPark.Application.Features.Estacionamentos.ListarEstacionamentos;

internal static class Distancia
{
    private const double RaioTerraKm = 6371.0;

    /// <summary>Distância em linha reta (fórmula de Haversine). Rotas reais ficam com o serviço de mapas externo.</summary>
    public static double EmKm(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        var dLat = ParaRadianos(latitude2 - latitude1);
        var dLon = ParaRadianos(longitude2 - longitude1);

        var a = Math.Pow(Math.Sin(dLat / 2), 2)
            + Math.Cos(ParaRadianos(latitude1)) * Math.Cos(ParaRadianos(latitude2)) * Math.Pow(Math.Sin(dLon / 2), 2);

        return RaioTerraKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ParaRadianos(double graus) => graus * Math.PI / 180.0;
}
