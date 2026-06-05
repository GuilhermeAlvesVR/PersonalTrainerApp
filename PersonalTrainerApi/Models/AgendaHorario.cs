namespace PersonalTrainerApi.Models;

public class AgendaHorario
{
    public int Id { get; set; }

    public string DiaSemana { get; set; } = string.Empty;

    public string Horario { get; set; } = string.Empty;

    public string Conteudo { get; set; } = string.Empty;
}
