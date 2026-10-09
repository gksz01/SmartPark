using System.Security.Claims;
using BlazorBlueprint.Components;
using SmartPark.Contracts.Autenticacao;

namespace SmartPark.Web.Comum;

/// <summary>Textos e cores de exibição para os valores que a API devolve como enum em texto.</summary>
public static class Rotulos
{
    public static (BadgeVariant, string) StatusReserva(string? status) => status switch
    {
        "Confirmada" => (BadgeVariant.SoftInfo, "Confirmada"),
        "Cancelada" => (BadgeVariant.SoftDestructive, "Cancelada"),
        "EmUso" => (BadgeVariant.SoftWarning, "Em uso"),
        "Concluida" => (BadgeVariant.SoftSuccess, "Concluída"),
        "Expirada" => (BadgeVariant.Secondary, "Expirada"),
        _ => (BadgeVariant.Outline, status ?? "-")
    };

    public static (BadgeVariant, string) StatusPagamento(string? status) => status switch
    {
        null => (BadgeVariant.Outline, "Não pago"),
        "Pendente" => (BadgeVariant.SoftWarning, "Pendente"),
        "Aprovado" => (BadgeVariant.SoftSuccess, "Pago"),
        "Recusado" => (BadgeVariant.SoftDestructive, "Recusado"),
        "Estornado" => (BadgeVariant.Secondary, "Estornado"),
        _ => (BadgeVariant.Outline, status)
    };

    public static (BadgeVariant, string) StatusVaga(string? status) => status switch
    {
        "Livre" => (BadgeVariant.SoftSuccess, "Livre"),
        "Ocupada" => (BadgeVariant.SoftDestructive, "Ocupada"),
        "Indisponivel" => (BadgeVariant.Secondary, "Indisponível"),
        _ => (BadgeVariant.Outline, status ?? "-")
    };

    public static string Perfil(ClaimsPrincipal usuario) =>
        usuario.IsInRole(Perfis.Administrador) ? "Administrador"
        : usuario.IsInRole(Perfis.Funcionario) ? "Funcionário / Manobrista"
        : "Motorista";
}
