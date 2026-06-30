using System;
using Microsoft.Maui.Controls;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.View;

public partial class HeaderBanner : ContentView
{
    public static readonly BindableProperty ShowBackButtonProperty =
        BindableProperty.Create(nameof(ShowBackButton), typeof(bool), typeof(HeaderBanner), false);

    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    public HeaderBanner()
    {
        InitializeComponent();
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler?.MauiContext != null)
        {
            var userService = Handler.MauiContext.Services.GetService<IUserService>();
            
            if (userService?.CurrentUser != null)
            {
                WelcomeLabel.Text = $"Welcome, {userService.CurrentUser.UserName}";
            }
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}