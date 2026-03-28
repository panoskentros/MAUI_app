using System;
using MAUI_app.Model;
using Microsoft.Maui.Controls;

namespace MAUI_app.View;

// This tells the page to look for the "PassedUser" parameter we just sent!
[QueryProperty(nameof(IncomingUser), "PassedUser")]
public partial class DashboardPage : ContentPage
{
    // When Shell passes the data, this property automatically updates the label
    public ApplicationUser IncomingUser
    {
        set { WelcomeLabel.Text = $"Welcome, {value.UserName}!"; }
    }

    // The constructor is now empty, so AppShell won't crash!
    public DashboardPage()
    {
        InitializeComponent();
    }
}