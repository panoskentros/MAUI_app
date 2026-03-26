using MAUI_app.Data;
using MAUI_app.Model;

namespace MAUI_app.Services;

using Microsoft.EntityFrameworkCore;

public interface IAuthService
{
    Task<ApplicationUser?> LoginAsync(string username,string email, string password);
}

public class AuthService : IAuthService
{
    private readonly IRepository<ApplicationUser> _userRepository;

    public AuthService(IRepository<ApplicationUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApplicationUser?> LoginAsync(string username, string email, string password)
    {
        var result = await _userRepository.GetAllAsync(u => 
            u.UserName == username && u.Email == email);

        if (!result.Success)
        {
            return null;
        }

        var user = result.Data.FirstOrDefault();

        if (user == null)
        {
            return null;
        }

        bool isValid = PasswordHasher.VerifyPassword(password, user.HashedPassword);

        return isValid ? user : null;
    }
}