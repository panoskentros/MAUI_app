using FluentValidation;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Services;

public class UserService : IUserService
{
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IValidator<ApplicationUser> _validator;
    
    public ApplicationUser? CurrentUser { get; private set; }
    
    public event EventHandler? UserChanged;
    
    public UserService(IRepository<ApplicationUser> userRepository, IValidator<ApplicationUser> validator)
    {
        _userRepository = userRepository;
        _validator = validator;
    }
    
    public bool IsLoggedIn => CurrentUser != null;
    
    private void OnUserChanged() => UserChanged?.Invoke(this, EventArgs.Empty);

    public async Task<IResult<ApplicationUser>> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _userRepository.GetQueryable().AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);

        if (user == null)
        {
            return Result<ApplicationUser>.Fail("Invalid username/email or password");
        }

        bool isValid = PasswordHasher.VerifyPassword(password, user.HashedPassword);

        if (isValid)
        {
            CurrentUser = user; 
            OnUserChanged();
            return Result<ApplicationUser>.Ok(user, string.Empty);
        }

        return Result<ApplicationUser>.Fail("Invalid username/email or password");
    }
    
    public async Task<List<ApplicationUser>> GetAllDoctorsAsync()
    {
        return await _userRepository.GetQueryable().AsNoTracking()
            .Where(u => u.Role == UserRole.Doctor)
            .ToListAsync();
    }
    
    public async Task<List<ApplicationUser>> GetAllPatientsAsync()
    {
        return await _userRepository.GetQueryable().AsNoTracking()
            .Where(u => u.Role == UserRole.Patient)
            .ToListAsync();
    }

    public async Task<IResult> RegisterAsync(ApplicationUser user)
    {
        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Fail(errors);
        }

        try
        {
            user.HashedPassword = PasswordHasher.HashPassword(user.HashedPassword);
            await _userRepository.AddAsync(user, asDetached: true);
            return Result.Ok(string.Empty);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public void Logout()
    {
        CurrentUser = null;
        OnUserChanged();
    }
}