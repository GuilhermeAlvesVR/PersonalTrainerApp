using ClosedXML.Excel;
using PersonalTrainerApp.Models;
using PersonalTrainerApp.Services;
using System.Globalization;

namespace PersonalTrainerApp.Views;

public partial class AgendaPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly List<string> _dias = new()
    {
        "Segunda-feira",
        "Terça-feira",
        "Quarta-feira",
        "Quinta-feira",
        "Sexta-feira"
    };

    private readonly List<string> _horarios = new()
    {
        "06:00",
        "07:00",
        "08:00",
        "09:00",
        "10:00",
        "15:00",
        "16:00",
        "17:00",
        "18:00",
        "19:00",
        "20:00"
    };

    private List<AgendaHorario> _agenda = new();
    private readonly List<CelulaAlunoAgenda> _celulasAgenda = new();
    private readonly Dictionary<int, List<Entry>> _camposHorario = new();
    private string? _alunoDestacado;
    private bool _atualizandoCamposHorario;

    private sealed class CelulaAlunoAgenda
    {
        public required AgendaHorario Item { get; init; }

        public required Entry Campo { get; init; }

        public required Border Celula { get; init; }
    }

    public AgendaPage(ApiService apiService)
    {
        InitializeComponent();

        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_agenda.Count == 0)
            await CarregarAgenda();
    }

    private async Task CarregarAgenda()
    {
        try
        {
            _agenda = await _apiService.GetAgendaHorariosAsync();

            if (_agenda.Count == 0)
                _agenda = CriarAgendaVazia();
            else
                AtualizarHorariosComAgendaSalva();

            MontarGradeAgenda();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar a agenda.\n{ex.Message}", "OK");
        }
    }

    private List<AgendaHorario> CriarAgendaVazia()
    {
        return _dias
            .SelectMany(dia => _horarios.Select(horario => new AgendaHorario
            {
                DiaSemana = dia,
                Horario = horario
            }))
            .ToList();
    }

    private void AtualizarHorariosComAgendaSalva()
    {
        var horariosSalvos = _agenda
            .Where(h => !string.IsNullOrWhiteSpace(h.Horario))
            .Select(h => h.Horario.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (horariosSalvos.Count == 0)
            return;

        _horarios.Clear();
        _horarios.AddRange(horariosSalvos);
    }

    private void MontarGradeAgenda()
    {
        AgendaGrid.Clear();
        AgendaGrid.RowDefinitions.Clear();
        AgendaGrid.ColumnDefinitions.Clear();
        _celulasAgenda.Clear();
        _camposHorario.Clear();
        AgendaGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        var linha = 0;

        foreach (var dia in _dias)
        {
            AgendaGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            AgendaGrid.Add(CriarBlocoDia(dia, linha == 0), 0, linha++);
        }

        AtualizarDestaques();
    }

    private Grid CriarBlocoDia(string dia, bool primeiroDia)
    {
        var itensDoDia = _horarios
            .Select(horario => ObterOuCriarHorario(dia, horario))
            .ToList();
        var linhasAlunos = Math.Max(5, itensDoDia.Max(item => ObterLinhasConteudo(item.Conteudo).Count) + 1);

        var bloco = new Grid
        {
            Margin = new Thickness(0, primeiroDia ? 0 : 12, 0, 0),
            RowSpacing = 0,
            ColumnSpacing = 0,
        };

        bloco.RowDefinitions.Add(new RowDefinition(new GridLength(38)));

        for (var linha = 0; linha < linhasAlunos; linha++)
            bloco.RowDefinitions.Add(new RowDefinition(new GridLength(44)));

        bloco.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(96)));

        foreach (var _ in _horarios)
            bloco.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(88)));

        var diaCell = CriarCelulaTexto(dia, Color.FromArgb("#111827"), Colors.White, 16, true);
        bloco.Add(diaCell, 0, 0);
        Grid.SetRowSpan(diaCell, linhasAlunos + 1);

        for (var indice = 0; indice < _horarios.Count; indice++)
        {
            bloco.Add(CriarCelulaHorarioEditavel(indice), indice + 1, 0);

            var item = itensDoDia[indice];

            for (var linhaAluno = 0; linhaAluno < linhasAlunos; linhaAluno++)
            {
                var cell = CriarCelulaAgenda(item, linhaAluno);
                bloco.Add(cell, indice + 1, linhaAluno + 1);
            }
        }

        return bloco;
    }

    private Border CriarCelulaHorarioEditavel(int indiceHorario)
    {
        var campo = new Entry
        {
            Text = _horarios[indiceHorario],
            MinimumHeightRequest = 34,
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#0B0F19"),
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            Margin = 0
        };

        campo.Completed += (_, _) => AplicarHorarioEditado(indiceHorario, campo);
        campo.Unfocused += (_, _) => AplicarHorarioEditado(indiceHorario, campo);

        if (!_camposHorario.TryGetValue(indiceHorario, out var campos))
        {
            campos = new List<Entry>();
            _camposHorario[indiceHorario] = campos;
        }

        campos.Add(campo);

        return new Border
        {
            BackgroundColor = Color.FromArgb("#4ADE80"),
            Stroke = Color.FromArgb("#374151"),
            StrokeThickness = 1,
            Padding = 2,
            Content = campo
        };
    }

    private void AplicarHorarioEditado(int indiceHorario, Entry campoEditado)
    {
        if (_atualizandoCamposHorario || indiceHorario < 0 || indiceHorario >= _horarios.Count)
            return;

        var horarioAntigo = _horarios[indiceHorario];
        var horarioNovo = (campoEditado.Text ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(horarioNovo))
        {
            campoEditado.Text = horarioAntigo;
            return;
        }

        if (string.Equals(horarioAntigo, horarioNovo, StringComparison.OrdinalIgnoreCase))
            return;

        foreach (var item in _agenda.Where(h => h.Horario == horarioAntigo))
            item.Horario = horarioNovo;

        _horarios[indiceHorario] = horarioNovo;

        if (!_camposHorario.TryGetValue(indiceHorario, out var campos))
            return;

        _atualizandoCamposHorario = true;

        foreach (var campo in campos)
            campo.Text = horarioNovo;

        _atualizandoCamposHorario = false;
    }

    private static Border CriarCelulaTexto(string texto, Color fundo, Color textoCor, double fonte, bool negrito)
    {
        return new Border
        {
            BackgroundColor = fundo,
            Stroke = Color.FromArgb("#374151"),
            StrokeThickness = 1,
            Padding = 4,
            Content = new Label
            {
                Text = texto,
                FontSize = fonte,
                FontAttributes = negrito ? FontAttributes.Bold : FontAttributes.None,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = textoCor
            }
        };
    }

    private Border CriarCelulaAgenda(AgendaHorario item, int linhaAluno)
    {
        var alunos = ObterLinhasConteudo(item.Conteudo);
        var campo = new Entry
        {
            Text = linhaAluno < alunos.Count ? alunos[linhaAluno] : string.Empty,
            MinimumHeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.White,
            Placeholder = string.Empty,
            PlaceholderColor = Color.FromArgb("#6B7280"),
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = 0
        };
        campo.TextChanged += (_, _) =>
        {
            AtualizarLinhaConteudo(item, linhaAluno, campo.Text ?? string.Empty);
            AtualizarDestaques();
        };

        var toqueCampo = new TapGestureRecognizer();
        toqueCampo.Tapped += (_, _) => SelecionarAlunoParaDestacar(campo.Text);
        campo.GestureRecognizers.Add(toqueCampo);

        var cell = new Border
        {
            BackgroundColor = Color.FromArgb("#0B0F19"),
            Stroke = Color.FromArgb("#374151"),
            StrokeThickness = 1,
            Padding = 2,
            Content = campo
        };

        var toque = new TapGestureRecognizer();
        toque.Tapped += (_, _) => SelecionarAlunoParaDestacar(campo.Text);
        cell.GestureRecognizers.Add(toque);

        _celulasAgenda.Add(new CelulaAlunoAgenda
        {
            Item = item,
            Campo = campo,
            Celula = cell
        });

        return cell;
    }

    private void SelecionarAlunoParaDestacar(string? aluno)
    {
        if (string.IsNullOrWhiteSpace(aluno))
            return;

        _alunoDestacado = aluno.Trim();
        LimparDestaqueButton.IsVisible = true;
        AtualizarDestaques();
    }

    private void AtualizarDestaques()
    {
        foreach (var celula in _celulasAgenda)
        {
            var destacado = !string.IsNullOrWhiteSpace(_alunoDestacado)
                && string.Equals(celula.Campo.Text?.Trim(), _alunoDestacado, StringComparison.OrdinalIgnoreCase);

            celula.Celula.BackgroundColor = Color.FromArgb(destacado ? "#3B2F0B" : "#0B0F19");
            celula.Celula.Stroke = Color.FromArgb(destacado ? "#FACC15" : "#374151");
            celula.Celula.StrokeThickness = destacado ? 2 : 1;
        }
    }

    private static List<string> ObterLinhasConteudo(string conteudo)
    {
        return conteudo
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n')
            .ToList();
    }

    private static void AtualizarLinhaConteudo(AgendaHorario item, int linha, string valor)
    {
        var linhas = ObterLinhasConteudo(item.Conteudo);

        while (linhas.Count <= linha)
            linhas.Add(string.Empty);

        linhas[linha] = valor.Trim();

        while (linhas.Count > 0 && string.IsNullOrWhiteSpace(linhas[^1]))
            linhas.RemoveAt(linhas.Count - 1);

        item.Conteudo = string.Join(Environment.NewLine, linhas);
    }

    private void LimparDestaque(object sender, EventArgs e)
    {
        _alunoDestacado = null;
        LimparDestaqueButton.IsVisible = false;
        AtualizarDestaques();
    }

    private AgendaHorario ObterOuCriarHorario(string dia, string horario)
    {
        var item = _agenda.FirstOrDefault(h => h.DiaSemana == dia && h.Horario == horario);

        if (item != null)
            return item;

        item = new AgendaHorario
        {
            DiaSemana = dia,
            Horario = horario
        };

        _agenda.Add(item);

        return item;
    }

    private async void ImportarExcel(object sender, EventArgs e)
    {
        try
        {
            var arquivo = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecione a planilha de horários",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                    { DevicePlatform.WinUI, new[] { ".xlsx" } },
                    { DevicePlatform.iOS, new[] { "org.openxmlformats.spreadsheetml.sheet" } },
                    { DevicePlatform.MacCatalyst, new[] { "org.openxmlformats.spreadsheetml.sheet" } }
                })
            });

            if (arquivo == null)
                return;

            await using var stream = await arquivo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var workbook = new XLWorkbook(memoryStream);
            var planilha = workbook.Worksheets.First();

            _agenda = ImportarAgenda(planilha);
            MontarGradeAgenda();

            await DisplayAlert("Importado", "Planilha carregada. Revise e toque em Salvar para enviar para a nuvem.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível importar a planilha.\n{ex.Message}", "OK");
        }
    }

    private List<AgendaHorario> ImportarAgenda(IXLWorksheet planilha)
    {
        var agenda = CriarAgendaVazia();

        foreach (var row in planilha.RowsUsed())
        {
            var textoDia = row.Cell(1).GetString();

            if (!textoDia.Contains("FEIRA", StringComparison.OrdinalIgnoreCase))
                continue;

            var dia = NormalizarDia(textoDia);

            var headerRow = Math.Max(1, row.RowNumber() - 2);
            var linhasConteudo = Enumerable.Range(row.RowNumber() - 1, 5)
                .Where(linha => linha > 0)
                .ToList();

            for (var coluna = 2; coluna <= 12; coluna++)
            {
                var horario = ObterHorario(planilha.Cell(headerRow, coluna));

                if (string.IsNullOrWhiteSpace(horario))
                    continue;

                var nomes = linhasConteudo
                    .Select(linha => planilha.Cell(linha, coluna).GetString().Trim())
                    .Where(valor => !string.IsNullOrWhiteSpace(valor))
                    .ToList();

                var item = agenda.FirstOrDefault(h => h.DiaSemana == dia && h.Horario == horario);

                if (item != null)
                    item.Conteudo = string.Join(Environment.NewLine, nomes);
            }
        }

        return agenda;
    }

    private static string NormalizarDia(string texto)
    {
        var normalizado = texto.Trim().ToUpperInvariant();

        if (normalizado.Contains("SEGUNDA")) return "Segunda-feira";
        if (normalizado.Contains("TER")) return "Terça-feira";
        if (normalizado.Contains("QUARTA")) return "Quarta-feira";
        if (normalizado.Contains("QUINTA")) return "Quinta-feira";
        if (normalizado.Contains("SEXTA")) return "Sexta-feira";

        return texto.Trim();
    }

    private static string ObterHorario(IXLCell cell)
    {
        if (cell.TryGetValue<DateTime>(out var data))
            return data.ToString("HH:mm", CultureInfo.InvariantCulture);

        if (cell.TryGetValue<double>(out var numero))
            return TimeSpan.FromDays(numero).ToString(@"hh\:mm", CultureInfo.InvariantCulture);

        return cell.GetString().Trim();
    }

    private async void Salvar(object sender, EventArgs e)
    {
        try
        {
            var salvo = await _apiService.SalvarAgendaHorariosAsync(_agenda);

            if (!salvo)
            {
                await DisplayAlert("Erro", "Não foi possível salvar a agenda na nuvem.", "OK");
                return;
            }

            await DisplayAlert("Salvo", "Agenda salva e disponível para os outros dispositivos.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível salvar a agenda.\n{ex.Message}", "OK");
        }
    }

    private async void Fechar(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
