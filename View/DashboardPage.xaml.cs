using System;
using MAUI_app.Model;
using Microsoft.Maui.Controls;

namespace MAUI_app.View;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(ApplicationUser user)
    {
        InitializeComponent();
        
        WelcomeLabel.Text = $"Welcome, {user.UserName}!";
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}