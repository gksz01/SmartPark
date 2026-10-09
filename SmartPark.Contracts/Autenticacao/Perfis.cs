namespace SmartPark.Contracts.Autenticacao;

/// <summary>Nomes dos perfis, como aparecem na claim de role do token.</summary>
public static class Perfis
{
    public const string Motorista = nameof(Motorista);
    public const string Funcionario = nameof(Funcionario);
    public const string Administrador = nameof(Administrador);
}
