using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.View;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using FluentValidation;
using MAUI_app.Controller;
using MAUI_app.Services.Interfaces;

#if WINDOWS
using Microsoft.Maui.LifecycleEvents;
#endif

namespace MAUI_app;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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
                    window.ExtendsContentIntoTitleBar = false;
                });
            });
        });
#endif
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IAppointmentService, AppointmentService>();
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<MyAppointmentsPage>();
        builder.Services.AddTransient<BookAppointmentController>();
        builder.Services.AddTransient<BookAppointmentPage>();
        builder.Services.AddTransient<AppointmentsController>();
        builder.Services.AddTransient<DashboardController>();
        builder.Services.AddTransient<MedicationsController>();
        builder.Services.AddTransient<MedicationsPage>();
        builder.Services.AddTransient<MessagesController>();
        builder.Services.AddTransient<MessagesPage>();
        builder.Services.AddTransient<SettingsController>();
        builder.Services.AddTransient<SettingsPage>();
        
        builder.Services.AddValidatorsFromAssemblyContaining<App>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}