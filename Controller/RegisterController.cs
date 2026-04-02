using FluentValidation;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.View;
using MAUI_app.Services;
using MAUI_app.View.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Controller;

public class RegisterController
{
    private readonly IRegisterView _view;
    private IRepository<ApplicationUser> _repository;
    private IValidator<ApplicationUser> _validator;

    public RegisterController(IRegisterView view, IRepository<ApplicationUser>  repository,IValidator<ApplicationUser> validator)
    {
        _view = view;
        _repository = repository;
        _validator = validator;
    }

    public async Task RegisterUserAsync(ApplicationUser user)
    { 
        _view.ClearErrors();
        
        var confirmPassword = _view.GetConfirmPassword();
        
        if (user.HashedPassword != confirmPassword)
        {
            _view.ShowFieldError(nameof(ApplicationUser.HashedPassword), "Passwords do not match");
            return;
        }
        
        var validationResult = await _validator.ValidateAsync(user);

        if (!validationResult.IsValid)
        {
           
            foreach (var error in validationResult.Errors)
            {
                _view.ShowFieldError(error.PropertyName, error.ErrorMessage);
            }
            return;
        }
        _view.SetLoading(true);
        try 
        {
            user.HashedPassword = PasswordHasher.HashPassword(user.HashedPassword);
            await _repository.AddAsync(user,asDetached:true);
            
            _view.ClearFields();
            _view.ClearErrors();
            await _view.ShowAlert("Success", "Account created successfully!");
            await _view.NavigateBack();
        }
        catch (DbUpdateException dbEx)
        {
            string realError = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
            await _view.ShowAlert("Database Error", realError);
        }
        catch (Exception ex)
        {
            await _view.ShowAlert("Error", "General Error: " + ex.Message);
        }
        finally 
        {
            _view.SetLoading(false);
        }
    }
}