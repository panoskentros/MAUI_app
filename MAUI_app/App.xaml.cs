using MAUI_app.Services.Interfaces;

namespace MAUI_app;

public partial class App : Application
{
    private readonly AppShell _shell;
    private readonly IUserService _userService;
    
    public App(AppShell shell, IUserService userService)
    {
        InitializeComponent();
        
        _shell = shell;
        _userService = userService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(_shell);
        
        window.Created += async (s, e) =>
        {
            bool isLoggedIn = await _userService.RestoreSessionAsync();
            
            if (isLoggedIn)
            {
                await Shell.Current.GoToAsync("//dashboard");
            }
        };

        return window;
    }
}