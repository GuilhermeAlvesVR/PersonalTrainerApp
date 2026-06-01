using PersonalTrainerApp.Models;
using PersonalTrainerApp.Services;

namespace PersonalTrainerApp.Views;

public partial class TreinosAlunoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly Aluno _aluno;

    public TreinosAlunoPage(Aluno aluno)
    {
        InitializeComponent();

        _apiService = new ApiService();
        _aluno = aluno;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarTreinos();
    }

    private async Task CarregarTreinos()
    {
        var treinos = await _apiService.GetTreinosAlunoAsync(_aluno.Id);

        TreinosCollection.ItemsSource = null;
        TreinosCollection.ItemsSource = treinos;
    }

    private async void AbrirTreino(object sender, EventArgs e)
    {
        var treino = new Treino
        {
            AlunoId = _aluno.Id,
            LetraTreino = "A",
            NomeTreino = string.Empty,
            Exercicios = new List<Exercicio>()
        };

        await Navigation.PushModalAsync(new TreinoPage(treino));
    }

    private async void EditarTreino(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is Treino treino)
        {
            await Navigation.PushModalAsync(new TreinoPage(treino));
        }
    }

    private async void ExcluirTreino(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is Treino treino)
        {
            bool confirmar = await DisplayAlert(
                "Excluir treino",
                $"Deseja excluir o treino {treino.LetraTreino}?",
                "Excluir",
                "Cancelar");

            if (!confirmar)
                return;

            bool excluiu = await _apiService.ExcluirTreinoAsync(treino.Id);

            if (!excluiu)
            {
                await DisplayAlert("Erro", "Não foi possível excluir o treino.", "OK");
                return;
            }

            await CarregarTreinos();
        }
    }

    private async void Fechar(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}