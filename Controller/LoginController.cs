using MAUI_app.Services;
using MAUI_app.View;
using MAUI_app.View.Interfaces;

namespace MAUI_app.Controller;

public class LoginController
{
    private readonly ILoginView _view;
    private readonly IAuthService _authService;

    public LoginController(ILoginView view, IAuthService authService)
    {
        _view = view;
        _authService = authService;
    }

    public async Task LoginAsync(string usernameOrEmail, string password)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            await _view.ShowAlert("Validation Error", "All fields are required");
            return;
        }

        _view.SetLoading(true);
    
        var user = await _authService.LoginAsync(usernameOrEmail, password);
    
        _view.SetLoading(false);

        if (user != null)
        {
            _view.ClearFields();
            await _view.NavigateToDashboard();
        }
        else
        {
            await _view.ShowAlert("Login Failed", "Invalid username/email or password");
        }
    }

    public async Task GoToRegisterAsync() => await _view.NavigateToRegister();

    public async Task LogoutAsync()
    {
        _view.SetLoading(true);
        _authService.Logout();
        await Task.Delay(300);
        _view.SetLoading(false);
        _view.ClearFields();
    }
}