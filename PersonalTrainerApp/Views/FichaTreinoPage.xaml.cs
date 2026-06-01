using PersonalTrainerApp.Models;
using PersonalTrainerApp.Services;

namespace PersonalTrainerApp.Views;

public partial class FichaTreinoPage : ContentPage
{
    private readonly ApiService _apiService;
    public Aluno AlunoSelecionadoAtual { get; set; }

    public FichaTreinoPage(Aluno aluno)
    {
        InitializeComponent();

        _apiService = new ApiService();
        AlunoSelecionadoAtual = aluno;

        BindingContext = this;
    }

    private void SelecionarTreinoA(object sender, EventArgs e)
    {
        SelecionarTreinoPorLetra("A");
    }

    private void SelecionarTreinoB(object sender, EventArgs e)
    {
        SelecionarTreinoPorLetra("B");
    }

    private void SelecionarTreinoC(object sender, EventArgs e)
    {
        SelecionarTreinoPorLetra("C");
    }

    private void SelecionarTreinoD(object sender, EventArgs e)
    {
        SelecionarTreinoPorLetra("D");
    }

    private void SelecionarTreinoPorLetra(string letra)
    {
        var treinoExistente = AlunoSelecionadoAtual.Treinos
            .FirstOrDefault(t => t.LetraTreino == letra);

        if (treinoExistente == null)
            return;

        AlunoSelecionadoAtual.TreinoSelecionado = treinoExistente;

        AtualizarTela();
    }

    private async void EditarTreino(object sender, EventArgs e)
    {
        var treino = AlunoSelecionadoAtual.TreinoSelecionado;

        if (treino == null)
        {
            treino = new Treino
            {
                AlunoId = AlunoSelecionadoAtual.Id,
                LetraTreino = "A",
                NomeTreino = string.Empty,
                Exercicios = new List<Exercicio>()
            };

            AlunoSelecionadoAtual.TreinoSelecionado = treino;
            AlunoSelecionadoAtual.Treinos.Add(treino);
        }

        var treinoPage = new TreinoPage(treino);

        await Navigation.PushModalAsync(treinoPage);

        treinoPage.Disappearing += async (s, args) =>
        {
            await RecarregarAluno();
        };
    }

    private async Task RecarregarAluno()
    {
        var treinos = await _apiService.GetTreinosAlunoAsync(AlunoSelecionadoAtual.Id);

        AlunoSelecionadoAtual.Treinos = treinos;

        AlunoSelecionadoAtual.TreinoSelecionado =
            treinos.FirstOrDefault(t => t.LetraTreino == AlunoSelecionadoAtual.TreinoSelecionado?.LetraTreino)
            ?? treinos.FirstOrDefault();

        AtualizarTela();
    }

    private async void SalvarTreinoFeito(object sender, EventArgs e)
    {
        if (AlunoSelecionadoAtual.TreinoSelecionado == null)
            return;

        var treino = AlunoSelecionadoAtual.TreinoSelecionado;

        AlunoSelecionadoAtual.UltimoTreinoRealizado =
            $"{treino.LetraTreino} - {treino.NomeTreino}";
        await _apiService.AtualizarUltimoTreinoAsync(
            AlunoSelecionadoAtual.Id,
            AlunoSelecionadoAtual.UltimoTreinoRealizado);

        AtualizarTela();

        MensagemSucesso.IsVisible = true;

        await Task.Delay(3000);

        MensagemSucesso.IsVisible = false;
    }

    private async void Fechar(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void AtualizarTela()
    {
        var alunoAtual = AlunoSelecionadoAtual;
        AlunoSelecionadoAtual = alunoAtual;

        BindingContext = null;
        BindingContext = this;
    }
}