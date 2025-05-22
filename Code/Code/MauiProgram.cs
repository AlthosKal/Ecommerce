using Ecommerce.Mobile.Services;
using Ecommerce.Mobile.ViewModels;
using Ecommerce.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar servicios
            builder.Services.AddSingleton<ApiService>();

            // Registrar ViewModels
            builder.Services.AddSingleton<CountriesViewModel>();

            // Registrar Páginas
            builder.Services.AddSingleton<MainPage>();

            // Configuración de la Shell
            builder.Services.AddSingleton<AppShell>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
