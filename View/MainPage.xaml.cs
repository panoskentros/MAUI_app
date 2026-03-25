using System;
using MAUI_app.Services;
using Microsoft.Maui.Controls;

namespace MAUI_app.View;

public partial class MainPage : ContentPage
{
    private readonly IAuthService _authService;

    public MainPage(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
    }

    private async void OnLoginClicked(object? sender, EventArgs eventArgs)
    {
        var username = UsernameEntry.Text;
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || 
            string.IsNullOrWhiteSpace(email) || 
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Validation", "Please fill in all fields.", "OK");
            return;
        }

        SetLoading(true);

        var user = await _authService.LoginAsync(username, email, password);

        SetLoading(false);

        if (user != null)
        {
            WelcomeLabel.Text = $"Welcome, {user.UserName}!";
            StatusLabel.Text = $"Authenticated as {user.Role}";
            ToggleLoginState(true);
        }
        else
        {
            await DisplayAlert("Login Failed", "Invalid credentials.", "OK");
            PasswordEntry.Text = string.Empty;
        }
    }
    
    private async void OnGoToRegisterClicked(object? sender, EventArgs e)
    {
        // Navigate to the RegisterPage and pass the existing auth service
        await Navigation.PushAsync(new RegisterPage(_authService));
    }

    private async void OnLogoutClickedAsync(object? sender, EventArgs e)
    {
        SetLoading(true);
        await Task.Delay(300);
        SetLoading(false);

        UsernameEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;

        WelcomeLabel.Text = "Business Login";
        StatusLabel.Text = "Please enter your credentials";

        ToggleLoginState(false);
    }

    private void ToggleLoginState(bool isLoggedIn)
    {
        UsernameEntry.IsVisible = !isLoggedIn;
        EmailEntry.IsVisible = !isLoggedIn;
        PasswordEntry.IsVisible = !isLoggedIn;
        LoginBtn.IsVisible = !isLoggedIn;
        LogoutBtn.IsVisible = isLoggedIn;
    }

    private void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        LoginBtn.IsEnabled = !isLoading;
    }

    private void OnExitClicked(object sender, EventArgs e) => Application.Current?.Quit();
}