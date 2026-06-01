namespace PersonalTrainerApp.Models;

public class HistoricoTreino
{
    public int Id { get; set; }

    public string AlunoId { get; set; } = string.Empty;

    public string TipoTreino { get; set; } = string.Empty;

    public string NomeTreino { get; set; } = string.Empty;

    public DateTime Data { get; set; }
}