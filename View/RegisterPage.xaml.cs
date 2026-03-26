using System;
using MAUI_app.Controller;
using MAUI_app.Data;
using MAUI_app.Model;

namespace MAUI_app.View;

public partial class RegisterPage : ContentPage, IRegisterView
{
    private readonly RegisterController _controller;
    private ApplicationUser _model;

    public RegisterPage(AppDbContext appDbContext)
    {
        InitializeComponent();
        
        _controller = new RegisterController(this, appDbContext);
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

    public void SetLoading(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        RegisterBtn.IsEnabled = !isLoading;
        BackToLoginBtn.IsEnabled = !isLoading;
    }

    public async Task ShowAlert(string title, string message)
    {
        await DisplayAlert(title, message, "OK");
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
}