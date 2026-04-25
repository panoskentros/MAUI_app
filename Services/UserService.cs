using FluentValidation;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IValidator<ApplicationUser> _validator;
    
    public ApplicationUser? CurrentUser { get; private set; }
    
    public event EventHandler? UserChanged;
    
    public UserService(AppDbContext dbContext, IValidator<ApplicationUser> validator)
    {
        _context = dbContext;
        _validator = validator;
    }
    
    public bool IsLoggedIn => CurrentUser != null;
    
    private void OnUserChanged() => UserChanged?.Invoke(this, EventArgs.Empty);
    public async Task<List<ApplicationUser>> GetAllDoctorsAsync()
    {
        return await _context.Set<ApplicationUser>().AsNoTracking()
            .Where(u => u.Role == UserRole.Doctor)
            .ToListAsync();
    }
    
    public async Task<ApplicationUser?> GetDoctorByIdAsync(int doctorId)
    {
        return await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Role == UserRole.Doctor && u.Id == doctorId);
    }
    
    public async Task<List<ApplicationUser>> GetAllPatientsAsync()
    {
        return await 
            _context.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Patient)
            .ToListAsync();
    }
    public async Task<ApplicationUser?> GetPatientByIdAsync(int doctorId)
    {
        return await 
            _context.Set<ApplicationUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Role == UserRole.Patient && u.Id == doctorId);
    }

    public async Task<Result<ApplicationUser>> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _context.Set<ApplicationUser>().AsNoTracking()
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
    public async Task<Result> RegisterAsync(ApplicationUser user)
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
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            
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