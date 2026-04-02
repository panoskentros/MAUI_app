using System;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services;
using MAUI_app.View.Interfaces;

namespace MAUI_app.View;

public partial class MainPage : ContentPage, IMainView
{
    private readonly MainController _controller;
    private readonly RegisterPage _registerPage;
    public MainPage(IAuthService authService,RegisterPage registerPage)
    {
        InitializeComponent();
        
        _controller = new MainController(this, authService);
        _registerPage = registerPage;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string usernameOrEmail = UsernameOrEmailEntry.Text;
        string password = PasswordEntry.Text;
        await _controller.LoginAsync(usernameOrEmail, password);
    }
    
    private async void OnGoToRegisterClicked(object sender, EventArgs e)
    {
        await _controller.GoToRegisterAsync();
    }

    private async void OnLogoutClickedAsync(object sender, EventArgs e)
    {
        await _controller.LogoutAsync();
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

    public void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        LoginBtn.IsEnabled = !isLoading;
        LoginBtn.Text = isLoading ? "" : "Sign In";
    }

    public async Task ShowAlert(string title, string message)
    {
        await DisplayAlert(title, message, "OK");
    }

    public async Task NavigateToDashboard()
    {
        await Shell.Current.GoToAsync("//dashboard");
    }

    public async Task NavigateToRegister()
    {
        await Navigation.PushAsync(_registerPage);
    }

    public void ClearFields()
    {
        UsernameOrEmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
    }
}