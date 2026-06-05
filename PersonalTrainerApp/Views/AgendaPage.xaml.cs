using ClosedXML.Excel;
using PersonalTrainerApp.Models;
using PersonalTrainerApp.Services;
using System.Collections.ObjectModel;
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
    private ObservableCollection<AgendaHorario> _agendaDia = new();

    public AgendaPage(ApiService apiService)
    {
        InitializeComponent();

        _apiService = apiService;
        DiasPicker.ItemsSource = _dias;
        AgendaCollection.ItemsSource = _agendaDia;
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

            DiasPicker.SelectedIndex = 0;
            AtualizarDiaSelecionado();
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

    private void SelecionarDia(object sender, EventArgs e)
    {
        AtualizarDiaSelecionado();
    }

    private void AtualizarDiaSelecionado()
    {
        if (DiasPicker.SelectedItem is not string dia)
            return;

        _agendaDia = new ObservableCollection<AgendaHorario>(
            _agenda
                .Where(h => h.DiaSemana == dia)
                .OrderBy(h => _horarios.IndexOf(h.Horario)));

        AgendaCollection.ItemsSource = _agendaDia;
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
            DiasPicker.SelectedIndex = 0;
            AtualizarDiaSelecionado();

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
