using PersonalTrainerApp.Models;
using PersonalTrainerApp.Services;

namespace PersonalTrainerApp.Views;

public partial class HistoricoAvaliacoesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly Aluno _aluno;

    public HistoricoAvaliacoesPage(Aluno aluno)
    {
        InitializeComponent();

        _apiService = new ApiService();
        _aluno = aluno;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var avaliacoes = await _apiService.GetAvaliacoesAlunoAsync(_aluno.Id);

        AvaliacoesCollection.ItemsSource = null;
        AvaliacoesCollection.ItemsSource = avaliacoes;
    }

    private async void Voltar(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}