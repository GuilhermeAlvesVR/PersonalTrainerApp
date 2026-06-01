using PersonalTrainerApp.Models;
using PersonalTrainerApp.Services;

namespace PersonalTrainerApp.Views;

public partial class AvaliacaoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly Aluno _aluno;

    public AvaliacaoPage(Aluno aluno)
    {
        InitializeComponent();

        _apiService = new ApiService();
        _aluno = aluno;
    }

    private async void SalvarAvaliacao(object sender, EventArgs e)
    {
        try
        {
            var avaliacao = new AvaliacaoFisica
            {
                AlunoId = _aluno.Id,
                Peso = string.IsNullOrWhiteSpace(PesoEntry.Text) ? 0 : Convert.ToDouble(PesoEntry.Text),
                Peito = string.IsNullOrWhiteSpace(PeitoEntry.Text) ? 0 : Convert.ToDouble(PeitoEntry.Text),
                Cintura = string.IsNullOrWhiteSpace(CinturaEntry.Text) ? 0 : Convert.ToDouble(CinturaEntry.Text),
                Abdomen = string.IsNullOrWhiteSpace(AbdomenEntry.Text) ? 0 : Convert.ToDouble(AbdomenEntry.Text),
                Quadril = string.IsNullOrWhiteSpace(QuadrilEntry.Text) ? 0 : Convert.ToDouble(QuadrilEntry.Text),
                Braco = string.IsNullOrWhiteSpace(BracoEntry.Text) ? 0 : Convert.ToDouble(BracoEntry.Text),
                Coxa = string.IsNullOrWhiteSpace(CoxaEntry.Text) ? 0 : Convert.ToDouble(CoxaEntry.Text),
                Observacoes = ObservacoesEditor.Text ?? string.Empty
            };

            var avaliacaoSalva = await _apiService.SalvarAvaliacaoAsync(avaliacao);

            if (avaliacaoSalva == null)
            {
                await DisplayAlert("Erro", "Não foi possível salvar a avaliação na nuvem.", "OK");
                return;
            }

            await DisplayAlert("Sucesso", "Avaliação salva na nuvem.", "OK");

            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao salvar", ex.Message, "OK");
        }
    }

    private async void Voltar(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}