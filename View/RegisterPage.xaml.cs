using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

public partial class RegisterPage : ContentPage, IRegisterView
{
    private readonly RegisterController _controller;

    public RegisterPage(IUserService userService)
    {
        InitializeComponent();
        _controller = new RegisterController(this, userService);
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        await _controller.HandleRegisterAsync(
            UsernameEntry.Text,
            EmailEntry.Text,
            PasswordEntry.Text,
            ConfirmPassword.Text
        );
    }

    private async void OnBackToLoginClicked(object? sender, EventArgs e)
    {
        await _controller.HandleBackToLoginAsync();
    }
    
    private async void OnToggleRegPasswordClicked(object? sender, TappedEventArgs e)
    {
        if (sender is Image eyeIcon)
        {
            _controller.HandleTogglePasswordClicked();
            
            eyeIcon.Source = PasswordEntry.IsPassword ? "eye_slash_icon.png" : "eye_icon.png";
            await eyeIcon.ScaleTo(0.8, 100);
            await eyeIcon.ScaleTo(1.0, 100);
        }
    }

    private async void OnToggleConfirmPasswordClicked(object? sender, TappedEventArgs e)
    {
        if (sender is Image eyeIcon)
        {
            _controller.HandleToggleConfirmPasswordClicked();
            
            eyeIcon.Source = ConfirmPassword.IsPassword ? "eye_slash_icon.png" : "eye_icon.png";
            await eyeIcon.ScaleTo(0.8, 100);
            await eyeIcon.ScaleTo(1.0, 100);
        }
    }

    private void OnConfirmPasswordCompleted(object? sender, EventArgs e)
    {
        OnRegisterClicked(sender, e);
    }

    public void SetUsernameError(string message, bool isVisible)
    {
        UsernameError.Text = message;
        UsernameError.IsVisible = isVisible;
    }

    public void SetEmailError(string message, bool isVisible)
    {
        EmailError.Text = message;
        EmailError.IsVisible = isVisible;
    }

    public void SetPasswordError(string message, bool isVisible)
    {
        PasswordError.Text = message;
        PasswordError.IsVisible = isVisible;
    }

    public void SetConfirmPasswordError(string message, bool isVisible)
    {
        ConfirmPasswordError.Text = message;
        ConfirmPasswordError.IsVisible = isVisible;
    }

    public void SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        RegisterBtn.IsEnabled = !isLoading;
        BackToLoginBtn.IsEnabled = !isLoading;
        RegisterBtn.Text = isLoading ? string.Empty : "Complete Registration";
    }

    public void ClearInputs()
    {
        UsernameEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        ConfirmPassword.Text = string.Empty;
    }

    public void UpdatePasswordVisibilityState(bool isHidden)
    {
        PasswordEntry.IsPassword = isHidden;
    }

    public void UpdateConfirmPasswordVisibilityState(bool isHidden)
    {
        ConfirmPassword.IsPassword = isHidden;
    }

    public Task ShowAlertAsync(string title, string message)
    {
        return DisplayAlert(title, message, "OK");
    }

    public Task NavigateBackAsync()
    {
        return Navigation.PopAsync();
    }
}