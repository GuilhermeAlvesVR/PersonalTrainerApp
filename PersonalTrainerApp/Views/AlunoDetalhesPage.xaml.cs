using PersonalTrainerApp.Models;
using PersonalTrainerApp.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace PersonalTrainerApp.Views;

public partial class AlunoDetalhesPage : ContentPage
{
    private readonly ApiService _apiService;

    public ISeries[] Series { get; set; } = Array.Empty<ISeries>();

    public Aluno Aluno { get; set; }

    public bool AlunoExcluido { get; private set; }

    public event EventHandler<Aluno>? AlunoExcluidoEvento;

    public AlunoDetalhesPage(Aluno aluno)
    {
        InitializeComponent();

        _apiService = new ApiService();

        Aluno = aluno;

        BindingContext = Aluno;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        BindingContext = null;
        BindingContext = Aluno;

        await CarregarAvaliacoes();
    }   

    private async Task CarregarAvaliacoes()
    {
        var avaliacoes = await _apiService.GetAvaliacoesAlunoAsync(Aluno.Id);

        Aluno.Avaliacoes = avaliacoes;
        CarregarGraficoPeso(avaliacoes);

        BotaoAvaliacao.Text = avaliacoes.Count == 0
            ? "Fazer avaliação"
            : "Nova avaliação";

        var treinos = await _apiService.GetTreinosAlunoAsync(Aluno.Id);
        TreinosCollection.ItemsSource = treinos;

        BindingContext = null;
        BindingContext = Aluno;
    }

    private async void AbrirAvaliacao(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AvaliacaoPage(Aluno));
    }

    private async void NovoTreino(object sender, EventArgs e)
    {
        var novoTreino = new Treino
        {
            AlunoId = Aluno.Id,
            LetraTreino = "A",
            NomeTreino = string.Empty
        };

        await Navigation.PushModalAsync(new TreinoPage(novoTreino));
    }

    private async void Fechar(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void ExcluirAluno(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert(
            "Excluir aluno",
            $"Deseja excluir {Aluno.Nome}?",
            "Excluir",
            "Cancelar");

        if (!confirmar)
            return;

        AlunoExcluido = true;

        AlunoExcluidoEvento?.Invoke(this, Aluno);

        await Navigation.PopModalAsync();
    }

    private async void EditarTreinos(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TreinosAlunoPage(Aluno));
    }

    private async void AbrirHistoricoAvaliacoes(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new HistoricoAvaliacoesPage(Aluno));
    }

    private void CarregarGraficoPeso(List<AvaliacaoFisica> avaliacoes)
    {
        var avaliacoesOrdenadas = avaliacoes
            .OrderBy(a => a.Data)
            .ToList();

        var pesos = avaliacoesOrdenadas
            .Select(a => a.Peso)
            .ToArray();

        var datas = avaliacoesOrdenadas
            .Select(a => a.Data.ToString("dd/MM"))
            .ToArray();

        GraficoPeso.Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = pesos,
                Fill = null,
                LineSmoothness = 0.8,
                GeometrySize = 9,
                GeometryFill = new SolidColorPaint(SKColor.Parse("#4ADE80")),
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#0F172A"), 3),
                Stroke = new SolidColorPaint(SKColor.Parse("#4ADE80"), 4)
            }
        };

        GraficoPeso.XAxes = new Axis[]
        {
            new Axis
            {
                Labels = datas,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#9CA3AF")),
                TextSize = 12,
                SeparatorsPaint = null
            }
        };

        GraficoPeso.YAxes = new Axis[]
        {
            new Axis
            {
                Labeler = value => $"{value:0}",
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#9CA3AF")),
                TextSize = 12,
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#1E293B"), 1)
            }
        };

        GraficoPeso.DrawMarginFrame = null;

        if (avaliacoesOrdenadas.Any())
        {
            var primeira = avaliacoesOrdenadas.First();
            var ultima = avaliacoesOrdenadas.Last();
            var diferenca = ultima.Peso - primeira.Peso;

            string sinal = diferenca > 0 ? "+" : "";

            PesoSelecionadoLabel.Text =
                $"Peso atual: {ultima.Peso} kg • Resultado: {sinal}{diferenca:0.#} kg";
        }
    }
    private async void EditarAluno(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(
            new EditarAlunoPage(Aluno));
    }
}