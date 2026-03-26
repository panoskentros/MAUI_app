using System;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services;

namespace MAUI_app.View;

public partial class MainPage : ContentPage, IMainView
{
    private readonly MainController _controller;
    private ApplicationUser _loginModel;
    private readonly RegisterPage _registerPage;
    public MainPage(IAuthService authService,RegisterPage registerPage)
    {
        InitializeComponent();
        
        _controller = new MainController(this, authService);
        _loginModel = new ApplicationUser();
        BindingContext = _loginModel;
        _registerPage = registerPage;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await _controller.LoginAsync(_loginModel.UserName, _loginModel.Email, _loginModel.HashedPassword);
    }

    private async void OnPasswordEntryCompleted(object sender, EventArgs e)
    {
        await _controller.LoginAsync(_loginModel.UserName, _loginModel.Email, _loginModel.HashedPassword);
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

    public void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        LoginBtn.IsEnabled = !isLoading;
    }

    public async Task ShowAlert(string title, string message)
    {
        await DisplayAlert(title, message, "OK");
    }

    public async Task NavigateToDashboard(ApplicationUser user)
    {
        await Navigation.PushAsync(new DashboardPage(user));
    }

    public async Task NavigateToRegister()
    {
        await Navigation.PushAsync(_registerPage);
    }

    public void ClearFields()
    {
        _loginModel =  new ApplicationUser();
        BindingContext = _loginModel;
    }
}