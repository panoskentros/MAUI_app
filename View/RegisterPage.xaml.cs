using System;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.View;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterController _controller;

    public RegisterPage(IUserService userService)
    {
        InitializeComponent();
        _controller = new RegisterController(userService);
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        UsernameError.IsVisible = false;
        EmailError.IsVisible = false;
        PasswordError.IsVisible = false;
        ConfirmPasswordError.IsVisible = false;

        bool hasError = false;

        if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
        {
            UsernameError.Text = "Username is required";
            UsernameError.IsVisible = true;
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            EmailError.Text = "Email is required";
            EmailError.IsVisible = true;
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            PasswordError.Text = "Password is required";
            PasswordError.IsVisible = true;
            hasError = true;
        }

        if (PasswordEntry.Text != ConfirmPassword.Text)
        {
            ConfirmPasswordError.Text = "Passwords do not match";
            ConfirmPasswordError.IsVisible = true;
            hasError = true;
        }

        if (hasError) return;

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        RegisterBtn.IsEnabled = false;
        BackToLoginBtn.IsEnabled = false;
        RegisterBtn.Text = "";

        var newUser = new ApplicationUser
        {
            UserName = UsernameEntry.Text,
            Email = EmailEntry.Text,
            HashedPassword = PasswordEntry.Text
        };

        var result = await _controller.RegisterUserAsync(newUser);

        LoadingIndicator.IsVisible = false;
        LoadingIndicator.IsRunning = false;
        RegisterBtn.IsEnabled = true;
        BackToLoginBtn.IsEnabled = true;
        RegisterBtn.Text = "Complete Registration";

        if (result.Success)
        {
            await DisplayAlert("Success", "Registration successful. Please log in.", "OK");
            UsernameEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            ConfirmPassword.Text = string.Empty;
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Registration Failed", result.Message, "OK");
        }
    }

    private async void OnBackToLoginClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    
    private bool _isRegPasswordHidden = true;
    private bool _isConfirmPasswordHidden = true;
    
    private async void OnToggleRegPasswordClicked(object? sender, TappedEventArgs e)
    {
        if (sender is Image eyeIcon)
        {
            _isRegPasswordHidden = !_isRegPasswordHidden;
            PasswordEntry.IsPassword = _isRegPasswordHidden;
            eyeIcon.Source = _isRegPasswordHidden ? "eye_slash_icon.png" : "eye_icon.png";
            await eyeIcon.ScaleTo(0.8, 100);
            await eyeIcon.ScaleTo(1.0, 100);
        }
    }

    private async void OnToggleConfirmPasswordClicked(object? sender, TappedEventArgs e)
    {
        if (sender is Image eyeIcon)
        {
            _isConfirmPasswordHidden = !_isConfirmPasswordHidden;
            ConfirmPassword.IsPassword = _isConfirmPasswordHidden;
            eyeIcon.Source = _isConfirmPasswordHidden ? "eye_slash_icon.png" : "eye_icon.png";
            await eyeIcon.ScaleTo(0.8, 100);
            await eyeIcon.ScaleTo(1.0, 100);
        }
    }

    private void OnConfirmPasswordCompleted(object? sender, EventArgs e)
    {
        OnRegisterClicked(sender, e);
    }
}