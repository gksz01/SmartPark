using SmartPark.Domain.Common;

namespace SmartPark.Domain.Pagamentos;

public static class PagamentoErrors
{
    public static readonly Error NaoEncontrado =
        Error.NotFound("Pagamento.NaoEncontrado", "Esta reserva ainda não possui pagamento.");

    public static readonly Error JaAprovado =
        Error.Conflict("Pagamento.JaAprovado", "O pagamento desta reserva já foi aprovado.");

    public static readonly Error NaoEstornavel =
        Error.Conflict("Pagamento.NaoEstornavel", "Somente pagamentos aprovados podem ser estornados.");

    public static readonly Error GatewayIndisponivel =
        Error.Unavailable("Pagamento.GatewayIndisponivel", "O serviço de pagamento está indisponível no momento. Tente novamente em instantes.");
}
