namespace PersonalTrainerApi.Models;

public class AvaliacaoFisica
{
    public int Id { get; set; }

    public DateTime Data { get; set; } = DateTime.UtcNow;

    public double Peso { get; set; }

    public double Peito { get; set; }

    public double Cintura { get; set; }

    public double Abdomen { get; set; }

    public double Quadril { get; set; }

    public double Braco { get; set; }

    public double Coxa { get; set; }

    public string Observacoes { get; set; } = string.Empty;

    public int AlunoId { get; set; }

    public Aluno? Aluno { get; set; }
}