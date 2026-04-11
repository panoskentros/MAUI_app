using System.Threading.Tasks;
using MAUI_app.Data;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class LoginController
{
    private readonly IUserService _userService;

    public LoginController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> LoginAsync(string usernameOrEmail, string password)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            return Result.Fail("Please enter both username/email and password");
        }

        var serviceResult = await _userService.LoginAsync(usernameOrEmail, password);

        if (serviceResult.Success)
        {
            return Result.Ok(serviceResult.Message);
        }

        return Result.Fail(serviceResult.Message);
    }
    public Result Logout()
    {
        _userService.Logout();
        return Result.Ok("Logged out successfully");
    }
}