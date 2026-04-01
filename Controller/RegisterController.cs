using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.View;
using MAUI_app.Services;
using MAUI_app.View.Interfaces;

namespace MAUI_app.Controller;

public class RegisterController
{
    private readonly IRegisterView _view;
    private readonly AppDbContext _context;

    public RegisterController(IRegisterView view, AppDbContext context)
    {
        _view = view;
        _context = context;
    }

    public async Task RegisterUserAsync(ApplicationUser user)
    {
        var confirmPassword = _view.GetConfirmPassword();

        if (string.IsNullOrWhiteSpace(user.UserName) || 
            string.IsNullOrWhiteSpace(user.Email) || 
            string.IsNullOrWhiteSpace(user.HashedPassword))
        {
            await _view.ShowAlert("Validation", "Please fill in all fields.");
            return;
        }

        if (user.HashedPassword != confirmPassword)
        {
            await _view.ShowAlert("Validation", "Passwords do not match.");
            return;
        }

        _view.SetLoading(true);
        
        try 
        {
            user.HashedPassword = PasswordHasher.HashPassword(user.HashedPassword);
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            await _view.ShowAlert("Success", "Account created successfully!");
            await _view.NavigateBack();
        }
        catch (Exception ex)
        {
            await _view.ShowAlert("Error", "Registration failed: " + ex.Message);
        }
        finally 
        {
            _view.SetLoading(false);
        }
    }
}