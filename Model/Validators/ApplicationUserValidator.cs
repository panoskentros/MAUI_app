using MAUI_app.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MAUI_app.Model.Validators;

public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public ApplicationUserValidator(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MustAsync(async (userInstance, username, cancellation) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
                var exists = await context.Users
                    .AnyAsync(x => x.UserName == username && x.Id != userInstance.Id, cancellation);
            
                return !exists;
            })
            .WithMessage("This username is already taken.");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(async (userInstance, email, cancellation) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
                var exists = await context.Users
                    .AnyAsync(x => x.Email == email && x.Id != userInstance.Id, cancellation);
            
                return !exists;
            })
            .WithMessage("An account with this email already exists.");

        RuleFor(x => x.HashedPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Please select a valid user role.");
    }
}