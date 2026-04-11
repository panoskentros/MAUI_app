using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.Data;

namespace MAUI_app.Controller;

public class RegisterController
{
    private readonly IUserService _userService;

    public RegisterController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> RegisterUserAsync(ApplicationUser user)
    {
        var registerResult = await _userService.RegisterAsync(user);

        if (registerResult.Success)
        {
            return Result.Ok(registerResult.Message ?? "Account created successfully!");
        }

        return Result.Fail(registerResult.Message ?? "Registration failed");
    }
}