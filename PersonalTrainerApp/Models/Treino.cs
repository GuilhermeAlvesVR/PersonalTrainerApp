using SQLite;

namespace PersonalTrainerApp.Models;

public class Treino
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string AlunoId { get; set; } = string.Empty;

    public string LetraTreino { get; set; } = "A";

    public string NomeTreino { get; set; } = string.Empty;

    [Ignore]
    public List<Exercicio> Exercicios { get; set; } = new();
}