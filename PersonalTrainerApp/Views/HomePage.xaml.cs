using PersonalTrainerApp.Models;
using System.Collections.ObjectModel;
using PersonalTrainerApp.Services;

namespace PersonalTrainerApp.Views;

public partial class HomePage : ContentPage
{
    public bool EhCelular => DeviceInfo.Idiom == DeviceIdiom.Phone;
    public bool NaoEhCelular => !EhCelular;
    private List<Aluno> TodosAlunos = new();
    private readonly ApiService _apiService;

    public ObservableCollection<Aluno> AlunosTreinando { get; set; } = new();

    private Aluno? alunoSelecionadoAtual;
    public Aluno? AlunoSelecionadoAtual
    {
        get => alunoSelecionadoAtual;
        set
        {
            alunoSelecionadoAtual = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemAlunoSelecionado));
        }
    }

    public bool TemAlunoSelecionado => AlunoSelecionadoAtual != null;

    public HomePage(ApiService apiService)
    {
        InitializeComponent();

        _apiService = apiService;

        ListaAlunos.ItemsSource = TodosAlunos;
        BindingContext = this;

        
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await CarregarAlunos();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao iniciar", ex.Message, "OK");
        }
    }

    private async Task CarregarAlunos()
    {
        try
        {
            var alunosBanco = await _apiService.GetAlunosAsync();

            TodosAlunos = alunosBanco;

            ListaAlunos.ItemsSource = null;
            ListaAlunos.ItemsSource = TodosAlunos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao carregar alunos", ex.Message, "OK");
        }
    }

    private async void SelecionarAlunoGeral(object sender, SelectionChangedEventArgs e)
    {
        var aluno = e.CurrentSelection.FirstOrDefault() as Aluno;

        if (aluno == null)
            return;

        bool jaEstaTreinando = AlunosTreinando.Any(a => a.Id == aluno.Id);

        if (!jaEstaTreinando)
            AlunosTreinando.Add(aluno);

        var treinos = await _apiService.GetTreinosAlunoAsync(aluno.Id);

        aluno.Treinos = treinos;
        aluno.TreinoSelecionado =
            treinos.FirstOrDefault(t => t.LetraTreino == "A")
            ?? treinos.FirstOrDefault();

        ListaAlunos.SelectedItem = null;
    }

    private async void SelecionarAlunoTreinando(object sender, SelectionChangedEventArgs e)
    {
        var aluno = e.CurrentSelection.FirstOrDefault() as Aluno;

        if (aluno == null)
            return;

        var treinos = await _apiService.GetTreinosAlunoAsync(aluno.Id);

        aluno.Treinos = treinos;
        aluno.TreinoSelecionado = treinos.FirstOrDefault();

        AlunoSelecionadoAtual = aluno;

        ListaAlunosTreinando.SelectedItem = null;
    }

    private async void AdicionarAluno(object sender, EventArgs e)
    {
        var paginaCadastro = new AlunoCadastroPage();

        await Navigation.PushModalAsync(paginaCadastro);

        paginaCadastro.Disappearing += async (s, args) =>
        {
            if (paginaCadastro.NovoAluno != null)
            {
                try
                {
                    var aluno = paginaCadastro.NovoAluno;

                    var alunoSalvo = await _apiService.SalvarAlunoAsync(aluno);

                    if (alunoSalvo == null)
                    {
                        await DisplayAlert("Erro", "Não foi possível salvar o aluno na nuvem.", "OK");
                        return;
                    }

                    foreach (var treino in aluno.Treinos)
                    {
                        treino.AlunoId = alunoSalvo.Id;

                        await _apiService.SalvarTreinoAsync(treino);
                    }

                    await CarregarAlunos();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro ao salvar", ex.Message, "OK");
                }
            }
        };
    }

    private void RemoverAlunoTreinando(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Aluno aluno)
        {
            aluno.TreinoEmAndamento = false;
            aluno.HoraInicioTreino = null;

            AlunosTreinando.Remove(aluno);

            if (AlunoSelecionadoAtual != null && AlunoSelecionadoAtual.Id == aluno.Id)
                AlunoSelecionadoAtual = null;
        }
    }
    private async void AbrirFichaAluno(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is Aluno aluno)
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                await Navigation.PushModalAsync(
                    new FichaTreinoPage(aluno));
            }
            else
            {
                AlunoSelecionadoAtual = aluno;
            }
        }
    }

    private void BuscarAluno(object sender, TextChangedEventArgs e)
    {
        string textoBusca = e.NewTextValue?.ToLower() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(textoBusca))
        {
            ListaAlunos.ItemsSource = TodosAlunos;
            return;
        }

        var alunosFiltrados = TodosAlunos
            .Where(a => a.Nome.ToLower().Contains(textoBusca))
            .ToList();

        ListaAlunos.ItemsSource = alunosFiltrados;
    }

    private async void EditarTreino(object sender, EventArgs e)
    {
        if (AlunoSelecionadoAtual == null)
            return;

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
            await RecarregarAlunoSelecionado();
        };
    }

    private async Task RecarregarAlunoSelecionado()
    {
        if (AlunoSelecionadoAtual == null)
            return;

        var alunoId = AlunoSelecionadoAtual.Id;

        var treinos = await _apiService.GetTreinosAlunoAsync(alunoId);

        AlunoSelecionadoAtual.Treinos = treinos;

        AlunoSelecionadoAtual.TreinoSelecionado =
            treinos.FirstOrDefault(t => t.LetraTreino == AlunoSelecionadoAtual.TreinoSelecionado?.LetraTreino)
            ?? treinos.FirstOrDefault();

        var alunoAtual = AlunoSelecionadoAtual;
        AlunoSelecionadoAtual = null;
        AlunoSelecionadoAtual = alunoAtual;
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
        if (AlunoSelecionadoAtual == null)
            return;

        var treinoExistente = AlunoSelecionadoAtual.Treinos
            .FirstOrDefault(t => t.LetraTreino == letra);

        if (treinoExistente == null)
            return;

        AlunoSelecionadoAtual.TreinoSelecionado = treinoExistente;

        var alunoAtual = AlunoSelecionadoAtual;
        AlunoSelecionadoAtual = null;
        AlunoSelecionadoAtual = alunoAtual;
    }

    private async void SalvarTreinoFeito(object sender, EventArgs e)
    {
        if (AlunoSelecionadoAtual == null ||
            AlunoSelecionadoAtual.TreinoSelecionado == null)
            return;

        var treino = AlunoSelecionadoAtual.TreinoSelecionado;

        AlunoSelecionadoAtual.UltimoTreinoRealizado =
            $"{treino.LetraTreino} - {treino.NomeTreino}";
        await _apiService.AtualizarUltimoTreinoAsync(
            AlunoSelecionadoAtual.Id,
            AlunoSelecionadoAtual.UltimoTreinoRealizado);

        var alunoAtual = AlunoSelecionadoAtual;
        AlunoSelecionadoAtual = null;
        AlunoSelecionadoAtual = alunoAtual;

        MensagemSucesso.IsVisible = true;

        await Task.Delay(3000);

        MensagemSucesso.IsVisible = false;
    }
    private async void VerDadosAluno(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Aluno aluno)
        {
            var page = new AlunoDetalhesPage(aluno);

            page.AlunoExcluidoEvento += async (s, alunoExcluido) =>
            {
                await _apiService.ExcluirAlunoAsync(alunoExcluido.Id);

                await CarregarAlunos();

                var alunoTreinando = AlunosTreinando
                    .FirstOrDefault(a => a.Id == alunoExcluido.Id);

                if (alunoTreinando != null)
                    AlunosTreinando.Remove(alunoTreinando);

                if (AlunoSelecionadoAtual?.Id == alunoExcluido.Id)
                    AlunoSelecionadoAtual = null;
            };

            await Navigation.PushModalAsync(page);
        }
    }

    private async void AdicionarAlunoTreinando(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.BindingContext is not Aluno aluno)
            return;

        bool jaExiste = AlunosTreinando.Any(a => a.Id == aluno.Id);

        if (!jaExiste)
            AlunosTreinando.Add(aluno);

        var treinos = await _apiService.GetTreinosAlunoAsync(aluno.Id);

        aluno.Treinos = treinos;

        aluno.TreinoSelecionado =
            treinos.FirstOrDefault(t => t.LetraTreino == "A")
            ?? treinos.FirstOrDefault();
    }
}