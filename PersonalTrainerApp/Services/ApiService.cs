    using System.Net.Http.Json;
    using PersonalTrainerApp.Models;

    namespace PersonalTrainerApp.Services;

    public class ApiService
    {
        private readonly HttpClient _httpClient;

        private const string BaseUrl = "http://192.168.15.186:5129/";

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    // ALUNOS
    public async Task<List<Aluno>> GetAlunosAsync()
    {
        var alunosApi = await _httpClient.GetFromJsonAsync<List<AlunoApiDto>>("api/Alunos");

        if (alunosApi == null)
            return new List<Aluno>();

        return alunosApi.Select(a => new Aluno
        {
            Id = a.Id.ToString(),
            Nome = a.Nome,
            Idade = a.Idade,
            Peso = a.Peso,
            Altura = a.Altura,
            Objetivo = a.Objetivo ?? string.Empty,
            Observacoes = a.Observacoes ?? string.Empty,
            Ativo = a.Ativo,
            UltimoTreinoRealizado = a.UltimoTreinoRealizado ?? string.Empty
        }).ToList();
    }

    public async Task<Aluno?> SalvarAlunoAsync(Aluno aluno)
    {
        var alunoApi = new AlunoApiDto
        {
            Nome = aluno.Nome,
            Idade = aluno.Idade,
            Peso = aluno.Peso,
            Altura = aluno.Altura,
            Objetivo = aluno.Objetivo,
            Observacoes = aluno.Observacoes,
            Ativo = aluno.Ativo,
            PersonalEmpresaId = 1
        };

        var response = await _httpClient.PostAsJsonAsync("api/Alunos", alunoApi);

        if (!response.IsSuccessStatusCode)
            return null;

        var salvo = await response.Content.ReadFromJsonAsync<AlunoApiDto>();

        if (salvo == null)
            return null;

        aluno.Id = salvo.Id.ToString();

        return aluno;
    }

    // TREINOS
    public async Task<List<Treino>> GetTreinosAlunoAsync(string alunoId)
    {
        var treinosApi = await _httpClient.GetFromJsonAsync<List<TreinoApiDto>>($"api/Treinos/aluno/{alunoId}");

        if (treinosApi == null)
            return new List<Treino>();

        return treinosApi.Select(t => new Treino
        {
            Id = t.Id.ToString(),
            AlunoId = t.AlunoId.ToString(),
            LetraTreino = t.LetraTreino,
            NomeTreino = t.NomeTreino,
            Exercicios = t.Exercicios.Select(e => new Exercicio
            {
                Id = e.Id.ToString(),
                TreinoId = e.TreinoId.ToString(),
                Nome = e.Nome,
                Series = e.Series,
                Repeticoes = e.Repeticoes,
                Carga = e.Carga
            }).ToList()
        }).ToList();
    }

    public async Task<Treino?> SalvarTreinoAsync(Treino treino)
    {
        var treinoApi = new TreinoApiDto
        {
            Id = int.TryParse(treino.Id, out var idTreino) ? idTreino : 0,
            AlunoId = int.Parse(treino.AlunoId),
            LetraTreino = treino.LetraTreino,
            NomeTreino = treino.NomeTreino,
            Exercicios = treino.Exercicios.Select(e => new ExercicioApiDto
            {
                Id = int.TryParse(e.Id, out var idExercicio) ? idExercicio : 0,
                Nome = e.Nome,
                Series = e.Series,
                Repeticoes = e.Repeticoes,
                Carga = e.Carga
            }).ToList()
        };

        var response = await _httpClient.PostAsJsonAsync("api/Treinos", treinoApi);

        if (!response.IsSuccessStatusCode)
            return null;

        var salvo = await response.Content.ReadFromJsonAsync<TreinoApiDto>();

        if (salvo == null)
            return null;

        treino.Id = salvo.Id.ToString();

        foreach (var exercicio in treino.Exercicios)
            exercicio.TreinoId = treino.Id;

        return treino;
    }
    // AVALIAÇÕES
    public async Task<AvaliacaoFisica?> SalvarAvaliacaoAsync(AvaliacaoFisica avaliacao)
    {
        var avaliacaoApi = new AvaliacaoApiDto
        {
            AlunoId = int.Parse(avaliacao.AlunoId),
            Peso = avaliacao.Peso,
            Peito = avaliacao.Peito,
            Cintura = avaliacao.Cintura,
            Abdomen = avaliacao.Abdomen,
            Quadril = avaliacao.Quadril,
            Braco = avaliacao.Braco,
            Coxa = avaliacao.Coxa,
            Observacoes = avaliacao.Observacoes
        };

        var response = await _httpClient.PostAsJsonAsync("api/Avaliacoes", avaliacaoApi);

        if (!response.IsSuccessStatusCode)
            return null;

        var salvo = await response.Content.ReadFromJsonAsync<AvaliacaoApiDto>();

        if (salvo == null)
            return null;

        avaliacao.Id = salvo.Id;
        avaliacao.Data = salvo.Data;

        return avaliacao;
    }

    public async Task<List<AvaliacaoFisica>> GetAvaliacoesAlunoAsync(string alunoId)
    {
        var avaliacoesApi =
            await _httpClient.GetFromJsonAsync<List<AvaliacaoApiDto>>($"api/Avaliacoes/aluno/{alunoId}");

        if (avaliacoesApi == null)
            return new List<AvaliacaoFisica>();

        return avaliacoesApi.Select(a => new AvaliacaoFisica
        {
            Id = a.Id,
            AlunoId = a.AlunoId.ToString(),
            Data = a.Data,
            Peso = a.Peso,
            Peito = a.Peito,
            Cintura = a.Cintura,
            Abdomen = a.Abdomen,
            Quadril = a.Quadril,
            Braco = a.Braco,
            Coxa = a.Coxa,
            Observacoes = a.Observacoes ?? string.Empty
        }).ToList();
    }
    public async Task<bool> ExcluirAlunoAsync(string alunoId)
    {
        var response = await _httpClient.DeleteAsync($"api/Alunos/{alunoId}");

        return response.IsSuccessStatusCode;
    }
    public async Task<bool> ExcluirTreinoAsync(string treinoId)
    {
        var response = await _httpClient.DeleteAsync($"api/Treinos/{treinoId}");

        return response.IsSuccessStatusCode;
    }
    public async Task<Aluno?> AtualizarAlunoAsync(Aluno aluno)
    {
        var alunoApi = new AlunoApiDto
        {
            Id = int.Parse(aluno.Id),
            Nome = aluno.Nome,
            Idade = aluno.Idade,
            Peso = aluno.Peso,
            Altura = aluno.Altura,
            Objetivo = aluno.Objetivo,
            Observacoes = aluno.Observacoes,
            Ativo = aluno.Ativo,
            UltimoTreinoRealizado = aluno.UltimoTreinoRealizado,
            PersonalEmpresaId = 1
        };

        var response = await _httpClient.PutAsJsonAsync($"api/Alunos/{aluno.Id}", alunoApi);

        if (!response.IsSuccessStatusCode)
            return null;

        var atualizado = await response.Content.ReadFromJsonAsync<AlunoApiDto>();

        if (atualizado == null)
            return null;

        aluno.Nome = atualizado.Nome;
        aluno.Idade = atualizado.Idade;
        aluno.Peso = atualizado.Peso;
        aluno.Altura = atualizado.Altura;
        aluno.Objetivo = atualizado.Objetivo ?? string.Empty;
        aluno.Observacoes = atualizado.Observacoes ?? string.Empty;
        aluno.Ativo = atualizado.Ativo;

        return aluno;
    }
    private class AlunoApiDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public double Peso { get; set; }
        public double Altura { get; set; }
        public string Objetivo { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public string UltimoTreinoRealizado { get; set; } = string.Empty;
        public int PersonalEmpresaId { get; set; }
    }

    private class TreinoApiDto
    {
        public int Id { get; set; }
        public string LetraTreino { get; set; } = string.Empty;
        public string NomeTreino { get; set; } = string.Empty;
        public int AlunoId { get; set; }
        public List<ExercicioApiDto> Exercicios { get; set; } = new();
    }

    private class ExercicioApiDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Series { get; set; }
        public int Repeticoes { get; set; }
        public string Carga { get; set; } = string.Empty;
        public int TreinoId { get; set; }
    }
    private class AvaliacaoApiDto
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }

        public DateTime Data { get; set; }

        public double Peso { get; set; }
        public double Peito { get; set; }
        public double Cintura { get; set; }
        public double Abdomen { get; set; }
        public double Quadril { get; set; }
        public double Braco { get; set; }
        public double Coxa { get; set; }

        public string Observacoes { get; set; } = string.Empty;
    }
    public async Task<bool> AtualizarUltimoTreinoAsync(string alunoId, string ultimoTreino)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/Alunos/{alunoId}/ultimo-treino",
            ultimoTreino);

        return response.IsSuccessStatusCode;
    }
}