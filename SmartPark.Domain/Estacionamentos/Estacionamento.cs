using System.Globalization;
using SmartPark.Domain.Common;
using SmartPark.Domain.Reservas;

namespace SmartPark.Domain.Estacionamentos;

public sealed class Estacionamento : AuditableEntity
{
    public const int MaximoVagasPorLote = 500;

    private readonly List<Vaga> _vagas = [];

    private Estacionamento()
    {
    }

    public string Nome { get; private set; } = null!;

    public string Endereco { get; private set; } = null!;

    public double? Latitude { get; private set; }

    public double? Longitude { get; private set; }

    public int CapacidadeTotal { get; private set; }

    /// <summary>Parte da capacidade que pode ser reservada antecipadamente; o restante atende quem chega sem reserva.</summary>
    public int CapacidadeReservavel { get; private set; }

    public decimal ValorHora { get; private set; }

    public bool Ativo { get; private set; }

    /// <summary>
    /// Token de concorrência otimista. Toda reserva confirmada troca o valor, então duas confirmações
    /// simultâneas no mesmo estacionamento não passam juntas: a segunda falha e revalida a capacidade.
    /// </summary>
    public Guid Versao { get; private set; }

    public IReadOnlyCollection<Vaga> Vagas => _vagas.AsReadOnly();

    public static Result<Estacionamento> Criar(
        string nome,
        string endereco,
        double? latitude,
        double? longitude,
        int capacidadeTotal,
        int capacidadeReservavel,
        decimal valorHora)
    {
        var validacao = Validar(nome, endereco, latitude, longitude, capacidadeTotal, capacidadeReservavel, valorHora);
        if (validacao.IsFailure)
        {
            return validacao.Error;
        }

        return new Estacionamento
        {
            Nome = nome.Trim(),
            Endereco = endereco.Trim(),
            Latitude = latitude,
            Longitude = longitude,
            CapacidadeTotal = capacidadeTotal,
            CapacidadeReservavel = capacidadeReservavel,
            ValorHora = valorHora,
            Ativo = true,
            Versao = Guid.NewGuid()
        };
    }

    /// <summary>Requer as vagas carregadas, para garantir que a capacidade não fique abaixo delas.</summary>
    public Result Atualizar(
        string nome,
        string endereco,
        double? latitude,
        double? longitude,
        int capacidadeTotal,
        int capacidadeReservavel,
        decimal valorHora)
    {
        var validacao = Validar(nome, endereco, latitude, longitude, capacidadeTotal, capacidadeReservavel, valorHora);
        if (validacao.IsFailure)
        {
            return validacao;
        }

        if (capacidadeTotal < _vagas.Count)
        {
            return EstacionamentoErrors.CapacidadeMenorQueVagas;
        }

        Nome = nome.Trim();
        Endereco = endereco.Trim();
        Latitude = latitude;
        Longitude = longitude;
        CapacidadeTotal = capacidadeTotal;
        CapacidadeReservavel = capacidadeReservavel;
        ValorHora = valorHora;
        Versao = Guid.NewGuid();

        return Result.Success();
    }

    public void Ativar() => Ativo = true;

    public void Desativar() => Ativo = false;

    public int CapacidadeDisponivel(Periodo periodo, IEnumerable<Periodo> reservasAtivas) =>
        Math.Max(0, CapacidadeReservavel - CalculadoraOcupacao.PicoSimultaneo(reservasAtivas, periodo));

    /// <summary>
    /// Confirma que ainda cabe uma reserva no período e marca o agregado como alterado, o que aciona o
    /// controle de concorrência na gravação.
    /// </summary>
    public Result GarantirCapacidade(Periodo periodo, IEnumerable<Periodo> reservasAtivas)
    {
        if (!Ativo)
        {
            return EstacionamentoErrors.Inativo;
        }

        if (CapacidadeDisponivel(periodo, reservasAtivas) <= 0)
        {
            return EstacionamentoErrors.CapacidadeEsgotada;
        }

        Versao = Guid.NewGuid();
        return Result.Success();
    }

    /// <summary>Cobrança por hora iniciada.</summary>
    public decimal CalcularValor(Periodo periodo) =>
        (decimal)Math.Ceiling(periodo.Duracao.TotalHours) * ValorHora;

    /// <summary>Requer as vagas carregadas. Numera a partir do maior número já usado com o mesmo prefixo.</summary>
    public Result<IReadOnlyList<Vaga>> AdicionarVagas(string prefixo, int quantidade)
    {
        prefixo = prefixo.Trim().ToUpperInvariant();

        if (prefixo.Length is < 1 or > 5 || !prefixo.All(char.IsAsciiLetterOrDigit))
        {
            return EstacionamentoErrors.PrefixoInvalido;
        }

        if (quantidade is < 1 or > MaximoVagasPorLote)
        {
            return EstacionamentoErrors.QuantidadeVagasInvalida;
        }

        if (_vagas.Count + quantidade > CapacidadeTotal)
        {
            return EstacionamentoErrors.VagasExcedemCapacidade;
        }

        var ultimoNumero = _vagas
            .Where(v => v.Identificador.StartsWith(prefixo, StringComparison.Ordinal))
            .Select(v => int.TryParse(v.Identificador.AsSpan(prefixo.Length), NumberStyles.None, CultureInfo.InvariantCulture, out var numero) ? numero : 0)
            .DefaultIfEmpty(0)
            .Max();

        List<Vaga> novas = [.. Enumerable.Range(ultimoNumero + 1, quantidade)
            .Select(numero => Vaga.Criar(Id, $"{prefixo}{numero:D2}"))];

        _vagas.AddRange(novas);
        return novas;
    }

    public Result AlterarStatusVaga(Guid vagaId, StatusVaga status)
    {
        var vaga = _vagas.SingleOrDefault(v => v.Id == vagaId);
        if (vaga is null)
        {
            return EstacionamentoErrors.VagaNaoEncontrada;
        }

        vaga.AlterarStatus(status);
        return Result.Success();
    }

    public Result RemoverVaga(Guid vagaId)
    {
        var vaga = _vagas.SingleOrDefault(v => v.Id == vagaId);
        if (vaga is null)
        {
            return EstacionamentoErrors.VagaNaoEncontrada;
        }

        if (vaga.Status == StatusVaga.Ocupada)
        {
            return EstacionamentoErrors.VagaOcupada;
        }

        _vagas.Remove(vaga);
        return Result.Success();
    }

    private static Result Validar(
        string nome,
        string endereco,
        double? latitude,
        double? longitude,
        int capacidadeTotal,
        int capacidadeReservavel,
        decimal valorHora)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return EstacionamentoErrors.NomeObrigatorio;
        }

        if (string.IsNullOrWhiteSpace(endereco))
        {
            return EstacionamentoErrors.EnderecoObrigatorio;
        }

        var coordenadasValidas = (latitude, longitude) switch
        {
            (null, null) => true,
            ({ } lat, { } lng) => lat is >= -90 and <= 90 && lng is >= -180 and <= 180,
            _ => false
        };

        if (!coordenadasValidas)
        {
            return EstacionamentoErrors.CoordenadasInvalidas;
        }

        if (capacidadeTotal <= 0)
        {
            return EstacionamentoErrors.CapacidadeTotalInvalida;
        }

        if (capacidadeReservavel < 0 || capacidadeReservavel > capacidadeTotal)
        {
            return EstacionamentoErrors.CapacidadeReservavelInvalida;
        }

        return valorHora < 0 ? EstacionamentoErrors.ValorHoraInvalido : Result.Success();
    }
}
