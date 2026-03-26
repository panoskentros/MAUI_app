using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.View;
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
        builder.Services.AddScoped(typeof(IRepository<>), typeof(PostgreRepository<>));
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<RegisterPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}