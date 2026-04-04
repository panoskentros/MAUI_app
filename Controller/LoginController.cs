using System.Threading.Tasks;
using MAUI_app.Services;
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
        _view.ClearErrors(); 

        bool hasError = false;

        if (string.IsNullOrWhiteSpace(usernameOrEmail))
        {
            _view.ShowFieldError("UsernameOrEmail", "Please enter your username or email");
            hasError = true;
        }
        
        if (string.IsNullOrWhiteSpace(password))
        {
            _view.ShowFieldError("Password", "Please enter your password");
            hasError = true;
        }

        if (hasError) return;

        _view.SetLoading(true);
    
        var result = await _authService.LoginAsync(usernameOrEmail, password);
    
        _view.SetLoading(false);

        if (result.Success)
        {
            _view.ClearFields();
            await _view.NavigateToDashboard();
        }
        else
        {
            _view.ShowFieldError("Password", result.Message ?? "Login failed"); 
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