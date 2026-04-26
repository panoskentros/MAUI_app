using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces; 
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

public partial class SettingsPage : ContentPage, ISettingsView
{
    private readonly SettingsController _controller;

    public SettingsPage(IUserService userService)
    {
        InitializeComponent();
        
        _controller = new SettingsController(this, userService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeDataAsync();
    }

    public void SetUserData(string username, string email)
    {
        UsernameEntry.Text = username;
        EmailEntry.Text = email;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        string newUsername = UsernameEntry.Text;
        string newEmail = EmailEntry.Text;
        string newPassword = PasswordEntry.Text;

        await _controller.HandleSaveProfileAsync(newUsername, newEmail, newPassword);
        PasswordEntry.Text = string.Empty;
    }

    private async void OnSignOutClicked(object? sender, EventArgs e)
    {
        await _controller.HandleSignOutAsync();
    }

    public Task ShowMessageAsync(string message, bool isError = false)
    {
        if (isError)
            return DisplayAlert("Error", message, "OK");
        else
            return DisplayAlert("Info", message, "OK");
    }

    public Task NavigateToLoginAsync()
    {
        return Shell.Current.GoToAsync("//LoginPage"); 
    }
}