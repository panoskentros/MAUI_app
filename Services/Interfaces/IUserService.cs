using MAUI_app.Data;
using MAUI_app.Model;

namespace MAUI_app.Services.Interfaces;

public interface IUserService
{
    ApplicationUser? CurrentUser { get; }
    bool IsLoggedIn { get; }
    event EventHandler? UserChanged;
    Task<IResult<ApplicationUser>> LoginAsync(string usernameOrEmail, string password);
    Task<IResult> RegisterAsync(ApplicationUser user);
    void Logout();
}
