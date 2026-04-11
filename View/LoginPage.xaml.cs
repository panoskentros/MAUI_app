using System;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.View;

public partial class LoginPage : ContentPage
{
    private readonly LoginController _controller;
    private readonly RegisterPage _registerPage;

    public LoginPage(IUserService userService, RegisterPage registerPage)
    {
        InitializeComponent();
        
        _controller = new LoginController(userService);
        _registerPage = registerPage;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        UsernameError.IsVisible = false;
        PasswordError.IsVisible = false;

        string usernameOrEmail = UsernameOrEmailEntry.Text;
        string password = PasswordEntry.Text;

        bool hasLocalError = false;

        if (string.IsNullOrWhiteSpace(usernameOrEmail))
        {
            UsernameError.Text = "Please enter your username or email";
            UsernameError.IsVisible = true;
            hasLocalError = true;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            PasswordError.Text = "Please enter your password";
            PasswordError.IsVisible = true;
            hasLocalError = true;
        }

        if (hasLocalError) return;

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        LoginBtn.IsEnabled = false;
        LoginBtn.Text = "";

        var result = await _controller.LoginAsync(usernameOrEmail, password);

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;
        LoginBtn.IsEnabled = true;
        LoginBtn.Text = "Sign In";

        if (result.Success)
        {
            UsernameOrEmailEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            await Shell.Current.GoToAsync("//dashboard");
        }
        else
        {
            PasswordError.Text = result.Message;
            PasswordError.IsVisible = true;
        }
    }
    
    private async void OnGoToRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(_registerPage);
    }

    private  void OnLogoutClicked(object sender, EventArgs e)
    {
        _controller.Logout();
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
        Application.Current?.Quit();
    }
    
    private bool _isPasswordHidden = true;
    
    private async void OnTogglePasswordClicked(object sender, TappedEventArgs e)
    {
        if (sender is Image eyeIcon)
        {
            _isPasswordHidden = !_isPasswordHidden;
            PasswordEntry.IsPassword = _isPasswordHidden;
            eyeIcon.Source = _isPasswordHidden ? "eye_slash_icon.png" : "eye_icon.png";
            await eyeIcon.ScaleTo(0.8, 100);
            await eyeIcon.ScaleTo(1.0, 100);
        }
    }
}