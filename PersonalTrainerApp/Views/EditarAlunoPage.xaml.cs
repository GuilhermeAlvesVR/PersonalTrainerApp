using PersonalTrainerApp.Models;
using PersonalTrainerApp.Services;

namespace PersonalTrainerApp.Views;

public partial class EditarAlunoPage : ContentPage
{
    private readonly ApiService _apiService;

    public Aluno Aluno { get; set; }

    public EditarAlunoPage(Aluno aluno)
    {
        InitializeComponent();

        _apiService = new ApiService();

        Aluno = aluno;

        BindingContext = Aluno;
    }

    private async void Salvar(object sender, EventArgs e)
    {
        var resultado = await _apiService.AtualizarAlunoAsync(Aluno);

        if (resultado == null)
        {
            await DisplayAlert(
                "Erro",
                "Não foi possível salvar o aluno.",
                "OK");

            return;
        }

        await DisplayAlert(
            "Sucesso",
            "Aluno atualizado.",
            "OK");

        await Navigation.PopModalAsync();
    }

    private async void Cancelar(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}