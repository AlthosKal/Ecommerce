using CommunityToolkit.Maui;
using FRONT_END.Service;
using FRONT_END.View;
using FRONT_END.ViewModels;
using Microsoft.Extensions.Logging;

namespace FRONT_END;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<CountryViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<AppShell>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
