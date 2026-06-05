using Microsoft.EntityFrameworkCore;
using PersonalTrainerApi.Models;

namespace PersonalTrainerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<PersonalEmpresa> PersonalEmpresas => Set<PersonalEmpresa>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Aluno> Alunos => Set<Aluno>();

    public DbSet<Treino> Treinos => Set<Treino>();

    public DbSet<Exercicio> Exercicios => Set<Exercicio>();

    public DbSet<AvaliacaoFisica> AvaliacoesFisicas => Set<AvaliacaoFisica>();

    public DbSet<AgendaHorario> AgendaHorarios => Set<AgendaHorario>();
}
