using MAUI_app.Data;
using MAUI_app.Model;

namespace MAUI_app.Services;

using Microsoft.EntityFrameworkCore;

public interface IAuthService
{
    
    ApplicationUser? CurrentUser { get; }
    bool IsLoggedIn { get; }
    event EventHandler? UserChanged;
    Task<ApplicationUser?> LoginAsync(string usernameOrEmail, string password);
    void Logout();
}

public class AuthService : IAuthService
{
    private readonly IRepository<ApplicationUser> _userRepository;
    public ApplicationUser? CurrentUser { get; private set; }
    public event EventHandler? UserChanged;
    public AuthService(IRepository<ApplicationUser> userRepository)
    {
        _userRepository = userRepository;
    }
    public bool IsLoggedIn => CurrentUser != null;
    private void OnUserChanged() => UserChanged?.Invoke(this, EventArgs.Empty);

    
    public async Task<ApplicationUser?> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _userRepository.GetQueryable().AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);

        if (user == null) return null;

        bool isValid = PasswordHasher.VerifyPassword(password, user.HashedPassword);

        if (isValid)
        {
            CurrentUser = user; 
            OnUserChanged();
            return user;
        }

        return null;
    }
    public void Logout()
    {
        CurrentUser = null;
    }
}