namespace PersonalTrainerApi.Models;

public class Treino
{
    public int Id { get; set; }

    public string LetraTreino { get; set; } = string.Empty;

    public string NomeTreino { get; set; } = string.Empty;

    public int AlunoId { get; set; }

    public Aluno? Aluno { get; set; }

    public List<Exercicio> Exercicios { get; set; } = new();
}