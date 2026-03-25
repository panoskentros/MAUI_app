namespace MAUI_app.Services;

using BC = BCrypt.Net.BCrypt;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        return BC.HashPassword(password);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BC.Verify(password, hashedPassword);
    }
}