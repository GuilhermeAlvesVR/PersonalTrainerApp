namespace PersonalTrainerApi.Models;

public class Aluno
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public int Idade { get; set; }

    public double Peso { get; set; }

    public double Altura { get; set; }

    public string Objetivo { get; set; } = string.Empty;

    public string Observacoes { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
    public string UltimoTreinoRealizado { get; set; } = string.Empty;

    public int PersonalEmpresaId { get; set; }

    public PersonalEmpresa? PersonalEmpresa { get; set; }

    public List<Treino> Treinos { get; set; } = new();

    public List<AvaliacaoFisica> Avaliacoes { get; set; } = new();
    
}