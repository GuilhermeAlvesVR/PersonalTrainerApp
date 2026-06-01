namespace PersonalTrainerApi.Models;

public class PersonalEmpresa
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public List<Usuario> Usuarios { get; set; } = new();

    public List<Aluno> Alunos { get; set; } = new();
}