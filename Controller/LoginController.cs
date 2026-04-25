using System.Threading.Tasks;
using MAUI_app.Data;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.Controller;

public class LoginController
{
    private readonly ILoginView _view;
    private readonly IUserService _userService;
    private bool _isPasswordHidden = true;

    public LoginController(ILoginView view, IUserService userService)
    {
        _view = view;
        _userService = userService;
    }

    public async Task HandleLoginAsync(string usernameOrEmail, string password)
    {
        _view.SetUsernameError(string.Empty, false);
        _view.SetPasswordError(string.Empty, false);

        bool hasLocalError = false;

        if (string.IsNullOrWhiteSpace(usernameOrEmail))
        {
            _view.SetUsernameError("Please enter your username or email", true);
            hasLocalError = true;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _view.SetPasswordError("Please enter your password", true);
            hasLocalError = true;
        }

        if (hasLocalError) return;

        _view.SetLoadingState(true);

        var serviceResult = await _userService.LoginAsync(usernameOrEmail, password);

        _view.SetLoadingState(false);

        if (serviceResult.Success)
        {
            _view.ClearInputs();
            await _view.NavigateToDashboardAsync();
        }
        else
        {
            _view.SetPasswordError(serviceResult.Message, true);
        }
    }

    public async Task HandleGoToRegisterClickedAsync()
    {
        await _view.NavigateToRegisterAsync();
    }

    public void HandleLogout()
    {
        _userService.Logout();
    }

    public void HandleExit()
    {
        _view.QuitApplication();
    }

    public void HandleTogglePasswordClicked()
    {
        _isPasswordHidden = !_isPasswordHidden;
        _view.UpdatePasswordVisibilityState(_isPasswordHidden);
    }
}