using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.View.Interfaces;

namespace MAUI_app.Controller;

public class RegisterController
{
    private readonly IRegisterView _view;
    private readonly IUserService _userService;

    public RegisterController(IRegisterView view, IUserService userService)
    {
        _view = view;
        _userService = userService;
    }

    public async Task RegisterUserAsync(ApplicationUser user)
    { 
        _view.ClearErrors();
        
        if (user.HashedPassword != _view.GetConfirmPassword())
        {
            _view.ShowFieldError(nameof(ApplicationUser.HashedPassword), "Passwords do not match");
            return;
        }

        _view.SetLoading(true);

        var registerResult = await _userService.RegisterAsync(user);

        _view.SetLoading(false);

        if (registerResult.Success)
        {
            _view.ClearFields();
            await _view.ShowAlert("Success", "Account created successfully!");
            await _view.NavigateBack();
        }
        else
        {
            await _view.ShowAlert("Registration Failed", registerResult.Message);
        }
    }
}