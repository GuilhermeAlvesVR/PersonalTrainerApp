using PersonalTrainerApp.Models;
using System.Collections.ObjectModel;

namespace PersonalTrainerApp.Views;

public partial class AlunoCadastroPage : ContentPage
{
    public Aluno? NovoAluno { get; private set; }

    private ObservableCollection<Exercicio> Exercicios { get; set; } = new();
    private Dictionary<string, ObservableCollection<Exercicio>> ExerciciosPorTreino = new();

    public AlunoCadastroPage()
    {
        InitializeComponent();

        LetraTreinoPicker.SelectedIndex = 0;

        ExerciciosPorTreino["A"] = new ObservableCollection<Exercicio>();

        Exercicios = ExerciciosPorTreino["A"];

        ListaExercicios.ItemsSource = Exercicios;
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

    private async void SalvarAluno(object sender, EventArgs e)
    {
        NovoAluno = new Aluno
        {
            Nome = NomeEntry.Text ?? string.Empty,
            Objetivo = ObjetivoEntry.Text ?? string.Empty,
            Observacoes = ObservacoesEditor.Text ?? string.Empty
        };

        int.TryParse(IdadeEntry.Text, out int idade);
        NovoAluno.Idade = idade;

        double.TryParse(PesoEntry.Text, out double peso);
        NovoAluno.Peso = peso;

        double.TryParse(AlturaEntry.Text, out double altura);
        NovoAluno.Altura = altura;

        if (CriarTreinoSwitch.IsToggled)
        {
            foreach (var treinoItem in ExerciciosPorTreino)
            {
                if (!treinoItem.Value.Any())
                    continue;

                var treino = new Treino
                {
                    LetraTreino = treinoItem.Key,
                    NomeTreino = NomeTreinoEntry.Text ?? string.Empty,
                    Exercicios = treinoItem.Value.ToList()
                };

                NovoAluno.Treinos.Add(treino);
            }

            NovoAluno.TreinoSelecionado = NovoAluno.Treinos.FirstOrDefault();
        }

        await Navigation.PopModalAsync();
    }
    private void CriarTreinoSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        TreinoContainer.IsVisible = e.Value;
    }
    private void RemoverExercicio(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Exercicio exercicio)
        {
            Exercicios.Remove(exercicio);
        }
    }
    private void LetraTreinoPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string letra = LetraTreinoPicker.SelectedItem?.ToString() ?? "A";

        if (!ExerciciosPorTreino.ContainsKey(letra))
        {
            ExerciciosPorTreino[letra] = new ObservableCollection<Exercicio>();
        }

        Exercicios = ExerciciosPorTreino[letra];

        ListaExercicios.ItemsSource = null;
        ListaExercicios.ItemsSource = Exercicios;
    }
    private async void Voltar(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}