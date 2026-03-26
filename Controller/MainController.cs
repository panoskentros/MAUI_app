using MAUI_app.Services;
using MAUI_app.View;

namespace MAUI_app.Controller;

public class MainController
{
    private readonly IMainView _view;
    private readonly IAuthService _authService;

    public MainController(IMainView view, IAuthService authService)
    {
        _view = view;
        _authService = authService;
    }

    public async Task LoginAsync(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await _view.ShowAlert("Validation Error", "All fields are required.");
            return;
        }

        _view.SetLoading(true);
        var user = await _authService.LoginAsync(username, email, password);
        _view.SetLoading(false);

        if (user != null)
        {
            _view.ClearFields();
            await _view.NavigateToDashboard(user);
        }
        else
        {
            await _view.ShowAlert("Login Failed", "Invalid username or password.");
        }
    }

    public async Task GoToRegisterAsync() => await _view.NavigateToRegister();

    public async Task LogoutAsync()
    {
        _view.SetLoading(true);
        await Task.Delay(300);
        _view.SetLoading(false);
        _view.ClearFields();
    }
}