using System;
using MAUI_app.Model;
using MAUI_app.Services;
using Microsoft.Maui.Controls;

namespace MAUI_app.View;

public partial class DashboardPage : ContentPage
{
    private ApplicationUser? _currentUser; 

    public DashboardPage(IAuthService authService)
    {
   
        InitializeComponent();    
        _currentUser = authService.CurrentUser;
        if (_currentUser != null)
        {
            WelcomeLabel.Text = $"Welcome, {_currentUser.UserName}!";
        }
    }
}