namespace SmartPark.Infrastructure.Persistencia;

public enum ProvedorBancoDados
{
    /// <summary>Padrão do desenvolvimento local (contêiner orquestrado pelo Aspire).</summary>
    PostgreSql,

    /// <summary>Provider dos demais ambientes.</summary>
    SqlServer
}
