using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.View;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

#if WINDOWS
using Microsoft.Maui.LifecycleEvents;
#endif

namespace MAUI_app;

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
#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windowsLifecycleBuilder =>
            {
                windowsLifecycleBuilder.OnWindowCreated(window =>
                {
                    // This single line forces the app to stay BELOW the Windows title bar!
                    window.ExtendsContentIntoTitleBar = false;
                });
            });
        });
#endif
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