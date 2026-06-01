using SQLite;

namespace PersonalTrainerApp.Models;

public class Exercicio
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string TreinoId { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public int Series { get; set; }

    public int Repeticoes { get; set; }

    public string Carga { get; set; } = string.Empty;
}