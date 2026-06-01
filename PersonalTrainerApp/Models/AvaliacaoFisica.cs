using SQLite;

namespace PersonalTrainerApp.Models;

public class AvaliacaoFisica
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string AlunoId { get; set; } = string.Empty;

    public DateTime Data { get; set; } = DateTime.Now;

    public double Peso { get; set; }

    public double Peito { get; set; }

    public double Cintura { get; set; }

    public double Abdomen { get; set; }

    public double Quadril { get; set; }

    public double Braco { get; set; }

    public double Coxa { get; set; }

    public string Observacoes { get; set; } = string.Empty;
}