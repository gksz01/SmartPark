using SmartPark.Domain.Common;

namespace SmartPark.Domain.Reservas;

public static class ReservaErrors
{
    public static readonly Error NaoEncontrada =
        Error.NotFound("Reserva.NaoEncontrada", "Reserva não encontrada.");

    public static readonly Error FimAntesDoInicio =
        Error.Validation("Reserva.FimAntesDoInicio", "O fim da reserva deve ser posterior ao início.");

    public static readonly Error InicioNoPassado =
        Error.Validation("Reserva.InicioNoPassado", "O início da reserva deve estar no futuro.");

    public static readonly Error DuracaoForaDoLimite =
        Error.Validation("Reserva.DuracaoForaDoLimite", "A reserva deve durar entre 30 minutos e 24 horas.");

    public static readonly Error AntecedenciaExcedida =
        Error.Validation("Reserva.AntecedenciaExcedida", "Reservas podem ser feitas com até 30 dias de antecedência.");

    public static readonly Error VeiculoInvalido =
        Error.Validation("Reserva.VeiculoInvalido", "Selecione um veículo ativo cadastrado em sua conta.");

    public static readonly Error NaoCancelavel =
        Error.Conflict("Reserva.NaoCancelavel", "Somente reservas confirmadas que ainda não começaram podem ser canceladas.");

    public static readonly Error NaoPagavel =
        Error.Conflict("Reserva.NaoPagavel", "Somente reservas confirmadas podem ser pagas.");
}
