using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.Controller;

public class RegisterController
{
    private readonly IRegisterView _view;
    private readonly IUserService _userService;
    private bool _isPasswordHidden = true;
    private bool _isConfirmPasswordHidden = true;

    public RegisterController(IRegisterView view, IUserService userService)
    {
        _view = view;
        _userService = userService;
    }

    public async Task HandleRegisterAsync(string username, string email, string password, string confirmPassword)
    {
        _view.SetUsernameError(string.Empty, false);
        _view.SetEmailError(string.Empty, false);
        _view.SetPasswordError(string.Empty, false);
        _view.SetConfirmPasswordError(string.Empty, false);

        bool hasError = false;

        if (string.IsNullOrWhiteSpace(username))
        {
            _view.SetUsernameError("Username is required", true);
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            _view.SetEmailError("Email is required", true);
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _view.SetPasswordError("Password is required", true);
            hasError = true;
        }

        if (password != confirmPassword)
        {
            _view.SetConfirmPasswordError("Passwords do not match", true);
            hasError = true;
        }

        if (hasError) return;

        _view.SetLoadingState(true);

        var newUser = new ApplicationUser
        {
            UserName = username,
            Email = email,
            HashedPassword = password
        };

        var registerResult = await _userService.RegisterAsync(newUser);

        _view.SetLoadingState(false);

        if (registerResult.Success)
        {
            await _view.ShowAlertAsync("Success", "Registration successful. Please log in.");
            _view.ClearInputs();
            await _view.NavigateBackAsync();
        }
        else
        {
            await _view.ShowAlertAsync("Registration Failed", registerResult.Message ?? "Registration failed");
        }
    }

    public async Task HandleBackToLoginAsync()
    {
        await _view.NavigateBackAsync();
    }

    public void HandleTogglePasswordClicked()
    {
        _isPasswordHidden = !_isPasswordHidden;
        _view.UpdatePasswordVisibilityState(_isPasswordHidden);
    }

    public void HandleToggleConfirmPasswordClicked()
    {
        _isConfirmPasswordHidden = !_isConfirmPasswordHidden;
        _view.UpdateConfirmPasswordVisibilityState(_isConfirmPasswordHidden);
    }
}