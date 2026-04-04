using System;
using FluentValidation;
using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class RegisterPage : ContentPage, IRegisterView
{
    
    private readonly RegisterController _controller;
    private ApplicationUser _model;

    public RegisterPage(IUserService  userService)
    {
        InitializeComponent();
        
        _controller = new RegisterController(this,userService);
        _model = new ApplicationUser();
        BindingContext = _model;
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        await _controller.RegisterUserAsync(_model);
    }

    private async void OnBackToLoginClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    
    private bool _isRegPasswordHidden = true;
    private bool _isConfirmPasswordHidden = true;
    private async void OnToggleRegPasswordClicked(object sender, TappedEventArgs e)
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

    private async void OnToggleConfirmPasswordClicked(object sender, TappedEventArgs e)
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

    public void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        RegisterBtn.IsEnabled = !isLoading;
        BackToLoginBtn.IsEnabled = !isLoading;
        RegisterBtn.Text = isLoading ? "" : "Complete Registration";
    }

    public async Task ShowAlert(string title, string message)
    {
        await DisplayAlert(title, message, "OK");
    }
    
    public void ClearErrors()
    {
        UsernameError.IsVisible = false;
        EmailError.IsVisible = false;
        PasswordError.IsVisible = false;
        ConfirmPasswordError.IsVisible = false;
    }

    public void ShowFieldError(string propertyName, string errorMessage)
    {
        switch (propertyName)
        {
            case "UserName":
                UsernameError.Text = errorMessage;
                UsernameError.IsVisible = true;
                break;
            case "Email":
                EmailError.Text = errorMessage;
                EmailError.IsVisible = true;
                break;
            case "HashedPassword":
                PasswordError.Text = errorMessage;
                PasswordError.IsVisible = true;
                break;
            case "ConfirmPassword":
                ConfirmPasswordError.Text = errorMessage;
                ConfirmPasswordError.IsVisible = true;
                break;
        }
    }
    

    public async Task NavigateBack()
    {
        await Navigation.PopAsync();
    }

    public string GetConfirmPassword()
    {
        return ConfirmPassword.Text ?? string.Empty;
    }
    private async void OnConfirmPasswordCompleted(object sender, EventArgs e)
    {
        await _controller.RegisterUserAsync(_model);
    }
    public void ClearFields()
    {
        _model = new ApplicationUser();
        ConfirmPassword.Text = string.Empty;
        BindingContext = _model;
    }
}