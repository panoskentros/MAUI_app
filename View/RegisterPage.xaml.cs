using System;
using MAUI_app.Services;
using Microsoft.Maui.Controls;

namespace MAUI_app.View;

public partial class RegisterPage : ContentPage
{
    private readonly IAuthService _authService;

    public RegisterPage(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
    }

    private async void OnRegisterClicked(object? sender, EventArgs eventArgs)
    {
        var username = UsernameEntry.Text;
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;
        var confirmPassword = ConfirmPasswordEntry.Text;

        // 1. Check for empty fields
        if (string.IsNullOrWhiteSpace(username) || 
            string.IsNullOrWhiteSpace(email) || 
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Validation", "Please fill in all fields.", "OK");
            return;
        }

        // 2. Validate passwords match
        if (password != confirmPassword)
        {
            await DisplayAlert("Validation", "Passwords do not match.", "OK");
            PasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;
            return;
        }

        SetLoading(true);

        // NOTE: You will need to make sure your IAuthService has a RegisterAsync method!
        // var success = await _authService.RegisterAsync(username, email, password);
        
        // Simulating network request for now
        await Task.Delay(1500); 
        bool success = true; // Replace with actual result from your auth service

        SetLoading(false);

        if (success)
        {
            await DisplayAlert("Success", "Account created successfully! You can now log in.", "OK");
            // Navigate back to the login page
            await Navigation.PopAsync(); 
        }
        else
        {
            await DisplayAlert("Registration Failed", "Could not create account. Please try again.", "OK");
        }
    }

    private async void OnBackToLoginClicked(object? sender, EventArgs e)
    {
        // Go back to the previous page (MainPage)
        await Navigation.PopAsync();
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        RegisterBtn.IsEnabled = !isLoading;
        BackToLoginBtn.IsEnabled = !isLoading;
    }
}