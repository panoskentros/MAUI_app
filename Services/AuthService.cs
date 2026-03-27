using MAUI_app.Data;
using MAUI_app.Model;

namespace MAUI_app.Services;

using Microsoft.EntityFrameworkCore;

public interface IAuthService
{
    Task<ApplicationUser?> LoginAsync(string usernameOrEmail, string password);
}

public class AuthService : IAuthService
{
    private readonly IRepository<ApplicationUser> _userRepository;

    public AuthService(IRepository<ApplicationUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApplicationUser?> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _userRepository.GetQueryable()
            .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);

        if (user == null) return null;

        bool isValid = PasswordHasher.VerifyPassword(password, user.HashedPassword);

        return isValid ? user : null;
    }
}