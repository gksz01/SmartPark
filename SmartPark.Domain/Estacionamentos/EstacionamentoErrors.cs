using SmartPark.Domain.Common;

namespace SmartPark.Domain.Estacionamentos;

public static class EstacionamentoErrors
{
    public static readonly Error NaoEncontrado =
        Error.NotFound("Estacionamento.NaoEncontrado", "Estacionamento não encontrado.");

    public static readonly Error Inativo =
        Error.Conflict("Estacionamento.Inativo", "Este estacionamento não está aceitando reservas.");

    public static readonly Error CapacidadeEsgotada =
        Error.Conflict("Estacionamento.CapacidadeEsgotada", "Capacidade esgotada para o período selecionado.");

    public static readonly Error NomeObrigatorio =
        Error.Validation("Estacionamento.NomeObrigatorio", "O nome é obrigatório.");

    public static readonly Error EnderecoObrigatorio =
        Error.Validation("Estacionamento.EnderecoObrigatorio", "O endereço é obrigatório.");

    public static readonly Error CoordenadasInvalidas =
        Error.Validation("Estacionamento.CoordenadasInvalidas", "Informe latitude (-90 a 90) e longitude (-180 a 180) juntas, ou nenhuma delas.");

    public static readonly Error CapacidadeTotalInvalida =
        Error.Validation("Estacionamento.CapacidadeTotalInvalida", "A capacidade total deve ser maior que zero.");

    public static readonly Error CapacidadeReservavelInvalida =
        Error.Validation("Estacionamento.CapacidadeReservavelInvalida", "A capacidade reservável deve estar entre zero e a capacidade total.");

    public static readonly Error ValorHoraInvalido =
        Error.Validation("Estacionamento.ValorHoraInvalido", "O valor por hora não pode ser negativo.");

    public static readonly Error CapacidadeMenorQueVagas =
        Error.Conflict("Estacionamento.CapacidadeMenorQueVagas", "A capacidade total não pode ser menor que o número de vagas cadastradas.");

    public static readonly Error VagasExcedemCapacidade =
        Error.Conflict("Estacionamento.VagasExcedemCapacidade", "O número de vagas não pode ultrapassar a capacidade total do estacionamento.");

    public static readonly Error PrefixoInvalido =
        Error.Validation("Estacionamento.PrefixoInvalido", "O prefixo deve ter de 1 a 5 letras ou números.");

    public static readonly Error QuantidadeVagasInvalida =
        Error.Validation("Estacionamento.QuantidadeVagasInvalida", "Informe de 1 a 500 vagas por lote.");

    public static readonly Error VagaNaoEncontrada =
        Error.NotFound("Estacionamento.VagaNaoEncontrada", "Vaga não encontrada.");

    public static readonly Error VagaOcupada =
        Error.Conflict("Estacionamento.VagaOcupada", "Não é possível remover uma vaga ocupada.");
}
