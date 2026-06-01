using SQLite;
using PersonalTrainerApp.Models;

namespace PersonalTrainerApp.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "personaltrainer.db");

        _database = new SQLiteAsyncConnection(dbPath);

        _database.CreateTableAsync<Aluno>().Wait();
        _database.CreateTableAsync<Treino>().Wait();
        _database.CreateTableAsync<Exercicio>().Wait();
        _database.CreateTableAsync<AvaliacaoFisica>().Wait();
    }

    // =========================
    // ALUNOS
    // =========================

    public async Task<List<Aluno>> GetAlunosAsync()
    {
        return await _database.Table<Aluno>().ToListAsync();
    }

    public async Task<int> SalvarAlunoAsync(Aluno aluno)
    {
        return await _database.InsertAsync(aluno);
    }

    // =========================
    // TREINOS
    // =========================

    public async Task<int> SalvarTreinoAsync(Treino treino)
    {
        return await _database.InsertOrReplaceAsync(treino);
    }

    public async Task<List<Treino>> GetTreinosAlunoAsync(string alunoId)
    {
        var treinos = await _database.Table<Treino>()
            .Where(t => t.AlunoId == alunoId)
            .ToListAsync();

        return treinos
            .GroupBy(t => t.LetraTreino)
            .Select(g => g.First())
            .OrderBy(t => t.LetraTreino)
            .ToList();
    }

    // =========================
    // EXERCÍCIOS
    // =========================

    public async Task<int> SalvarExercicioAsync(Exercicio exercicio)
    {
        return await _database.InsertOrReplaceAsync(exercicio);
    }

    public async Task<List<Exercicio>> GetExerciciosTreinoAsync(string treinoId)
    {
        return await _database.Table<Exercicio>()
            .Where(e => e.TreinoId == treinoId)
            .ToListAsync();
    }
    public async Task<int> ExcluirAlunoAsync(Aluno aluno)
    {
        return await _database.DeleteAsync(aluno);
    }
    public async Task<int> SalvarAvaliacaoAsync(AvaliacaoFisica avaliacao)
    {
        return await _database.InsertAsync(avaliacao);
    }

    public async Task<List<AvaliacaoFisica>> GetAvaliacoesAlunoAsync(string alunoId)
    {
        return await _database.Table<AvaliacaoFisica>()
            .Where(a => a.AlunoId == alunoId)
            .OrderByDescending(a => a.Data)
            .ToListAsync();
    }
    public async Task<int> ExcluirTreinoAsync(Treino treino)
    {
        return await _database.DeleteAsync(treino);
    }

    public async Task ExcluirExerciciosDoTreinoAsync(string treinoId)
    {
        var exercicios = await GetExerciciosTreinoAsync(treinoId);

        foreach (var exercicio in exercicios)
        {
            await _database.DeleteAsync(exercicio);
        }
    }
    public async Task<int> ExcluirExercicioAsync(Exercicio exercicio)
    {
        return await _database.DeleteAsync(exercicio);
    }
}