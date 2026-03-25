using System;
using MAUI_app.Model;
using Microsoft.Maui.Controls;

namespace MAUI_app.View;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(ApplicationUser user)
    {
        InitializeComponent();
        
        // Dynamically set the welcome text using the logged-in user's name
        WelcomeLabel.Text = $"Welcome, {user.UserName}!";
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        // Pop this page off the stack to go back to the login screen
        await Navigation.PopAsync();
    }
}