using MAUI_app.Model;
using MAUI_app.Services;
using Microsoft.Extensions.Logging;

namespace MAUI_app;

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
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddDbContext<AppDbContext>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}