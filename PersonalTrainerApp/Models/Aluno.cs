using SQLite;

namespace PersonalTrainerApp.Models;

public class Aluno
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Nome { get; set; } = string.Empty;

    public int Idade { get; set; }

    public double Peso { get; set; }

    public double Altura { get; set; }

    public double Peito { get; set; }
    [Ignore]
    public List<HistoricoTreino> HistoricoTreinos { get; set; } = new();

    public double Braco { get; set; }

    public double Cintura { get; set; }

    public double Coxa { get; set; }

    public double Panturrilha { get; set; }

    public string Objetivo { get; set; } = string.Empty;

    public string Observacoes { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
    public string UltimoTreinoRealizado { get; set; } = string.Empty;

    public bool TreinoEmAndamento { get; set; }

    public DateTime? HoraInicioTreino { get; set; }

    [Ignore]
    public List<Treino> Treinos { get; set; } = new();

    [Ignore]
    public Treino? TreinoSelecionado { get; set; }

    [Ignore]
    public bool TemTreino =>
        TreinoSelecionado != null &&
        TreinoSelecionado.Exercicios != null &&
        TreinoSelecionado.Exercicios.Any();

    [Ignore]
    public bool SemTreino => !TemTreino;
    [Ignore]
    public bool TemTreinoA =>
    Treinos.Any(t => t.LetraTreino == "A");

    [Ignore]
    public bool TemTreinoB =>
        Treinos.Any(t => t.LetraTreino == "B");

    [Ignore]
    public bool TemTreinoC =>
        Treinos.Any(t => t.LetraTreino == "C");

    [Ignore]
    public bool TemTreinoD =>
        Treinos.Any(t => t.LetraTreino == "D");

    [Ignore]
    public bool TreinoASelecionado =>
        TreinoSelecionado?.LetraTreino == "A";

    [Ignore]
    public bool TreinoBSelecionado =>
        TreinoSelecionado?.LetraTreino == "B";

    [Ignore]
    public bool TreinoCSelecionado =>
        TreinoSelecionado?.LetraTreino == "C";
    [Ignore]
    public bool TreinoDSelecionado =>
    TreinoSelecionado?.LetraTreino == "D";


    [Ignore]
    public List<AvaliacaoFisica> Avaliacoes { get; set; } = new();

    [Ignore]
    public AvaliacaoFisica? UltimaAvaliacao =>
        Avaliacoes
            .OrderByDescending(a => a.Data)
            .FirstOrDefault();


    [Ignore]
    public AvaliacaoFisica? AvaliacaoAnterior =>
    Avaliacoes
        .OrderByDescending(a => a.Data)
        .Skip(1)
        .FirstOrDefault();

    [Ignore]
    public bool TemEvolucao =>
        UltimaAvaliacao != null && AvaliacaoAnterior != null;

    [Ignore]
    public string EvolucaoPeitoTexto =>
        !TemEvolucao ? "Sem comparação" : FormatarEvolucao(UltimaAvaliacao!.Peito - AvaliacaoAnterior!.Peito, "cm");

    [Ignore]
    public string EvolucaoCinturaTexto =>
        !TemEvolucao ? "Sem comparação" : FormatarEvolucao(UltimaAvaliacao!.Cintura - AvaliacaoAnterior!.Cintura, "cm");

    [Ignore]
    public string EvolucaoAbdomenTexto =>
        !TemEvolucao ? "Sem comparação" : FormatarEvolucao(UltimaAvaliacao!.Abdomen - AvaliacaoAnterior!.Abdomen, "cm");

    [Ignore]
    public string EvolucaoQuadrilTexto =>
        !TemEvolucao ? "Sem comparação" : FormatarEvolucao(UltimaAvaliacao!.Quadril - AvaliacaoAnterior!.Quadril, "cm");

    [Ignore]
    public string EvolucaoBracoTexto =>
        !TemEvolucao ? "Sem comparação" : FormatarEvolucao(UltimaAvaliacao!.Braco - AvaliacaoAnterior!.Braco, "cm");

    [Ignore]
    public string EvolucaoCoxaTexto =>
        !TemEvolucao ? "Sem comparação" : FormatarEvolucao(UltimaAvaliacao!.Coxa - AvaliacaoAnterior!.Coxa, "cm");

    private string FormatarEvolucao(double valor, string unidade)
    {
        if (valor > 0)
            return $"↑ +{valor} {unidade}";

        if (valor < 0)
            return $"↓ {valor} {unidade}";

        return $"= 0 {unidade}";
    }
}