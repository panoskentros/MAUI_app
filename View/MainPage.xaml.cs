using System;
using MAUI_app.Model;
using MAUI_app.Services;
using Microsoft.Maui.Controls;

namespace MAUI_app.View;

public partial class MainPage : ContentPage
{
    private ApplicationUser loginModel { get; set; }
    private readonly IAuthService _authService;
    private readonly AppDbContext _appDbContext;

    public MainPage(IAuthService authService, AppDbContext appDbContext)
    {
        InitializeComponent();
        
        _authService = authService;
        _appDbContext = appDbContext;
        
        // Create a model to hold the login data and bind it to the UI
        loginModel = new ApplicationUser();
        BindingContext = loginModel;
    }

    private async void OnLoginClicked(object? sender, EventArgs eventArgs)
    {
        // Read directly from the bound model instead of the UI elements
        var username = loginModel.UserName;
        var email = loginModel.Email;
        var password = loginModel.HashedPassword;

        if (string.IsNullOrWhiteSpace(username) || 
            string.IsNullOrWhiteSpace(email) || 
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Validation", "Please fill in all fields.", "OK");
            return;
        }

        SetLoading(true);

        // This calls your PostgreSQL database via the AuthService
        var user = await _authService.LoginAsync(username, email, password);

        SetLoading(false);

        if (user != null)
        {
            // Clear the password field so it's empty if they log out
            loginModel.HashedPassword = string.Empty; 
            PasswordEntry.Text = string.Empty; 
            
            // Navigate to the new DashboardPage, passing the logged-in user object
            await Navigation.PushAsync(new DashboardPage(user));
        }
        else
        {
            await DisplayAlert("Login Failed", "Invalid credentials.", "OK");
            loginModel.HashedPassword = string.Empty; 
            PasswordEntry.Text = string.Empty;
        }
    }
    
    private async void OnGoToRegisterClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_authService, _appDbContext));
    }

    private async void OnLogoutClickedAsync(object? sender, EventArgs e)
    {
        SetLoading(true);
        await Task.Delay(300);
        SetLoading(false);

        UsernameEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        
        WelcomeLabel.Text = "Patient Portal";
        StatusLabel.Text = "Log in to manage your appointments";

        ToggleLoginState(false);
    }
    
    private void OnPasswordEntryCompleted(object sender, EventArgs e)
    {
        // When they hit Enter on the password field, just call the login button method!
        OnLoginClicked(sender, e);
    }

    private void ToggleLoginState(bool isLoggedIn)
    {
        UsernameEntry.IsVisible = !isLoggedIn;
        EmailEntry.IsVisible = !isLoggedIn;
        PasswordEntry.IsVisible = !isLoggedIn;
        LoginBtn.IsVisible = !isLoggedIn;
        GoToRegisterBtn.IsVisible = !isLoggedIn;
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