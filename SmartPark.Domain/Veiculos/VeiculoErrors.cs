using SmartPark.Domain.Common;

namespace SmartPark.Domain.Veiculos;

public static class VeiculoErrors
{
    public static readonly Error PlacaObrigatoria =
        Error.Validation("Veiculo.PlacaObrigatoria", "A placa é obrigatória.");

    public static readonly Error PlacaInvalida =
        Error.Validation("Veiculo.PlacaInvalida", "Placa inválida. Use o padrão ABC1234 ou ABC1D23.");

    public static readonly Error PlacaEmUso =
        Error.Conflict("Veiculo.PlacaEmUso", "Esta placa já está cadastrada em um veículo ativo.");

    public static readonly Error NaoEncontrado =
        Error.NotFound("Veiculo.NaoEncontrado", "Veículo não encontrado.");

    public static readonly Error PossuiReservaAtiva =
        Error.Conflict("Veiculo.PossuiReservaAtiva", "O veículo possui reservas ativas e não pode ser removido.");
}
