using MAUI_app.Model;

namespace MAUI_app.Services;

using Microsoft.EntityFrameworkCore;

public interface IAuthService
{
    Task<ApplicationUser?> LoginAsync(string username,string email, string password);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> LoginAsync(string username,string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == username && u.Email == email);

        if (user == null)
        {
            return null;
        }

        bool isValid = PasswordHasher.VerifyPassword(password, user.HashedPassword);

        return isValid ? user : null;
    }

   
}