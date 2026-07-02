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
        // 1. Για τα Pickers (Patient, Doctor)
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("CustomPickerWindows", (handler, view) =>
        {
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            handler.PlatformView.MinHeight = 0;
            handler.PlatformView.MinWidth = 0;
            handler.PlatformView.Padding = new Microsoft.UI.Xaml.Thickness(0);
        });

        // 2. Για το DatePicker
        Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("CustomDatePickerWindows", (handler, view) =>
        {
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            handler.PlatformView.MinHeight = 0;
            handler.PlatformView.MinWidth = 0;
            handler.PlatformView.Padding = new Microsoft.UI.Xaml.Thickness(0);
        });

        // 3. Για το TimePicker
        Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping("CustomTimePickerWindows", (handler, view) =>
        {
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            handler.PlatformView.MinHeight = 0;
            handler.PlatformView.MinWidth = 0;
            handler.PlatformView.Padding = new Microsoft.UI.Xaml.Thickness(0);
        });

        // 4. Για το Editor (Reason for Visit)
        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("CustomEditorWindows", (handler, view) =>
        {
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            // Αφαιρεί το native μπλε περίγραμμα των Windows όταν κάνεις click (Focus)
            handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
        });
#endif
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IAppointmentService, AppointmentService>();
        builder.Services.AddDbContext<AppDbContext>();
        
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        
        builder.Services.AddTransient<BookAppointmentController>();
        builder.Services.AddTransient<BookAppointmentPage>();
        
        builder.Services.AddTransient<AppointmentsController>();
        builder.Services.AddTransient<MyAppointmentsPage>();
        
        builder.Services.AddTransient<DashboardController>();
        builder.Services.AddTransient<DashboardPage>();
        
        builder.Services.AddTransient<SettingsController>();
        builder.Services.AddTransient<SettingsPage>();
        
        builder.Services.AddTransient<MedicationsController>();
        builder.Services.AddTransient<MedicationsPage>();
        
        builder.Services.AddTransient<AddMedicationController>();
        builder.Services.AddTransient<AddMedicationPage>();
        
        builder.Services.AddSingleton<AppShellController>();
        builder.Services.AddSingleton<AppShell>();
        
        builder.Services.AddValidatorsFromAssemblyContaining<App>();
        
        builder.Services.AddSingleton<IPreferences>(Preferences.Default);
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}