using PersonalTrainerApp.Models;
using System.Collections.ObjectModel;
using PersonalTrainerApp.Services;

namespace PersonalTrainerApp.Views;

public partial class TreinoPage : ContentPage
{
    private readonly ApiService _apiService;

    public Treino TreinoEditando { get; set; } = new();

    public ObservableCollection<Exercicio> Exercicios { get; set; } = new();

    public TreinoPage(Treino? treinoExistente = null)
    {
        InitializeComponent();

        _apiService = new ApiService();

        ListaExercicios.ItemsSource = Exercicios;

        if (treinoExistente != null)
        {
            TreinoEditando = treinoExistente;

            NomeTreinoEntry.Text = treinoExistente.NomeTreino;

            LetraTreinoPicker.SelectedItem =
                treinoExistente.LetraTreino;

            foreach (var exercicio in treinoExistente.Exercicios)
            {
                Exercicios.Add(exercicio);
            }
        }
    }

    private void EditarExercicio(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is Exercicio exercicio)
        {
            NomeExercicioEntry.Text = exercicio.Nome;
            SeriesEntry.Text = exercicio.Series.ToString();
            RepeticoesEntry.Text = exercicio.Repeticoes.ToString();
            CargaEntry.Text = exercicio.Carga;

            Exercicios.Remove(exercicio);
        }
    }

    private void AdicionarExercicio(object sender, EventArgs e)
    {
        int.TryParse(SeriesEntry.Text, out int series);
        int.TryParse(RepeticoesEntry.Text, out int repeticoes);

        var exercicio = new Exercicio
        {
            Nome = NomeExercicioEntry.Text ?? string.Empty,
            Series = series,
            Repeticoes = repeticoes,
            Carga = CargaEntry.Text ?? string.Empty
        };

        Exercicios.Add(exercicio);

        NomeExercicioEntry.Text = string.Empty;
        SeriesEntry.Text = string.Empty;
        RepeticoesEntry.Text = string.Empty;
        CargaEntry.Text = string.Empty;
    }

    private async void SalvarTreino(object sender, EventArgs e)
    {
        try
        {
            TreinoEditando.LetraTreino =
                LetraTreinoPicker.SelectedItem?.ToString() ?? "A";

            TreinoEditando.NomeTreino =
                NomeTreinoEntry.Text ?? string.Empty;

            TreinoEditando.Exercicios =
                Exercicios.ToList();

            

            var treinoSalvo = await _apiService.SalvarTreinoAsync(TreinoEditando);

            if (treinoSalvo == null)
            {
                await DisplayAlert(
                    "Erro",
                    "Não foi possível salvar o treino na nuvem.",
                    "OK");

                return;
            }

            await DisplayAlert(
                "Sucesso",
                "Treino salvo na nuvem com sucesso.",
                "OK");

            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erro ao salvar treino",
                ex.Message,
                "OK");
        }
    }

    private async void RemoverExercicio(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is Exercicio exercicio)
        {
            bool confirmar = await DisplayAlert(
                "Remover exercício",
                $"Deseja remover {exercicio.Nome}?",
                "Remover",
                "Cancelar");

            if (!confirmar)
                return;

            Exercicios.Remove(exercicio);
        }
    }

    private async void TrocarTreinoSelecionado(object sender, EventArgs e)
    {
        if (LetraTreinoPicker.SelectedItem == null)
            return;

        string letra = LetraTreinoPicker.SelectedItem.ToString() ?? "A";

        if (string.IsNullOrWhiteSpace(TreinoEditando.AlunoId))
            return;

        var treinosAluno =
            await _apiService.GetTreinosAlunoAsync(TreinoEditando.AlunoId);

        var treinoSelecionado = treinosAluno
            .FirstOrDefault(t => t.LetraTreino == letra);

        if (treinoSelecionado == null)
        {
            TreinoEditando = new Treino
            {
                AlunoId = TreinoEditando.AlunoId,
                LetraTreino = letra,
                NomeTreino = string.Empty,
                Exercicios = new List<Exercicio>()
            };

            NomeTreinoEntry.Text = string.Empty;
            Exercicios.Clear();

            return;
        }

        TreinoEditando = treinoSelecionado;

        NomeTreinoEntry.Text = treinoSelecionado.NomeTreino;

        Exercicios.Clear();

        foreach (var exercicio in treinoSelecionado.Exercicios)
        {
            Exercicios.Add(exercicio);
        }
    }

    private async void VoltarPagina(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}