using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

public partial class LoginPage : ContentPage, ILoginView
{
    private readonly LoginController _controller;
    private readonly RegisterPage _registerPage;

    public LoginPage(IUserService userService, RegisterPage registerPage)
    {
        InitializeComponent();
        
        _registerPage = registerPage;
        _controller = new LoginController(this, userService);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await _controller.HandleLoginAsync(UsernameOrEmailEntry.Text, PasswordEntry.Text);
    }
    
    private async void OnGoToRegisterClicked(object sender, EventArgs e)
    {
        await _controller.HandleGoToRegisterClickedAsync();
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        _controller.HandleLogout();
    }

    private void OnExitClicked(object sender, EventArgs e)
    {
        _controller.HandleExit();
    }
    
    private async void OnTogglePasswordClicked(object sender, TappedEventArgs e)
    {
        if (sender is Image eyeIcon)
        {
            _controller.HandleTogglePasswordClicked();
            
            eyeIcon.Source = PasswordEntry.IsPassword ? "eye_slash_icon.png" : "eye_icon.png";
            await eyeIcon.ScaleTo(0.8, 100);
            await eyeIcon.ScaleTo(1.0, 100);
        }
    }

    public void SetUsernameError(string message, bool isVisible)
    {
        UsernameError.Text = message;
        UsernameError.IsVisible = isVisible;
    }

    public void SetPasswordError(string message, bool isVisible)
    {
        PasswordError.Text = message;
        PasswordError.IsVisible = isVisible;
    }

    public void SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        LoginBtn.IsEnabled = !isLoading;
        LoginBtn.Text = isLoading ? string.Empty : "Sign In";
    }

    public void ClearInputs()
    {
        UsernameOrEmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
    }

    public void UpdatePasswordVisibilityState(bool isHidden)
    {
        PasswordEntry.IsPassword = isHidden;
    }

    public async Task NavigateToDashboardAsync()
    {
        await Shell.Current.GoToAsync("//dashboard");
    }

    public async Task NavigateToRegisterAsync()
    {
        await Navigation.PushAsync(_registerPage);
    }

    public void QuitApplication()
    {
        Application.Current?.Quit();
    }
}