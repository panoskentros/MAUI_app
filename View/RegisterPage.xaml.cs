using System;
using MAUI_app.Model;
using MAUI_app.Services;
using Microsoft.Maui.Controls;

namespace MAUI_app.View;

public partial class RegisterPage : ContentPage
{
    private ApplicationUser model { get; set; } 
    private readonly IAuthService _authService;
    private readonly AppDbContext _appDbContext;

    public RegisterPage(IAuthService authService, AppDbContext appDbContext)
    {
        InitializeComponent();
        
        _authService = authService;
        _appDbContext = appDbContext;
        
        model = new ApplicationUser();
        BindingContext = model; 
    }

    private async void OnRegisterClicked(object? sender, EventArgs eventArgs)
    {      
        var username = model.UserName;
        var email = model.Email;
         var password = model.HashedPassword;
         
         if (string.IsNullOrWhiteSpace(username) || 
             string.IsNullOrWhiteSpace(email) || 
             string.IsNullOrWhiteSpace(password))
         {
             await DisplayAlert("Validation", "Please fill in all fields.", "OK");
             return;
         }
         
         if (password != ConfirmPassword.Text)
         {
             await DisplayAlert("Validation", "Passwords do not match.", "OK");
             return;
         }
  
        SetLoading(true);
        await Task.Delay(1500);
        bool success = true;

        SetLoading(false);

        if (success)
        {
            model.HashedPassword = PasswordHasher.HashPassword(password);
            await _appDbContext.Users.AddAsync(model);
            await _appDbContext.SaveChangesAsync();
            await DisplayAlert("Success", "Account created successfully!", "OK");
            await Navigation.PopAsync();
        }
    }
    private async void OnBackToLoginClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    
    private void OnConfirmPasswordCompleted(object sender, EventArgs e)
    {
        // When they hit Enter on the confirm password field, trigger the sign up!
        OnRegisterClicked(sender, e);
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        RegisterBtn.IsEnabled = !isLoading;
        BackToLoginBtn.IsEnabled = !isLoading;
    }
}