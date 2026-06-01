using System.Text.Json.Serialization;

namespace PersonalTrainerApi.Models;

public class Exercicio
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public int Series { get; set; }

    public int Repeticoes { get; set; }

    public string Carga { get; set; } = string.Empty;

    public int TreinoId { get; set; }

    [JsonIgnore]
    public Treino? Treino { get; set; }
}