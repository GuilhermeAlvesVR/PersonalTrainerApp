using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using PersonalTrainerApp.Services;
using LiveChartsCore.SkiaSharpView.Maui;

namespace PersonalTrainerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .UseLiveCharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            
            builder.Services.AddSingleton<ApiService>();

            return builder.Build();
        }
    }
}