namespace PersonalTrainerApp.Models;

public class SessaoTreino
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string AlunoId { get; set; } = string.Empty;

    public string NomeAluno { get; set; } = string.Empty;

    public DateTime HoraInicio { get; set; } = DateTime.Now;

    public DateTime? HoraFim { get; set; }

    public bool EmAndamento { get; set; } = true;

    public string Observacoes { get; set; } = string.Empty;
}