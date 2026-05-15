using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MAUI_app.Controller;
using MAUI_app.Model;
using MAUI_app.View.interfaces;

namespace MAUI_app.View;

public partial class MessagesPage : ContentPage, IMessagesView
{
    private readonly MessagesController _controller;
    public MessagesPage()
    {
        InitializeComponent();
        
        _controller = new MessagesController(this);
        Title = "Inbox";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _controller.InitializeDataAsync();
    }
    public void SetMessages(List<MessageItem> messages)
    {
        MessagesList.ItemsSource = messages;
    }
    public Task ShowErrorAsync(string message)
    {
        return DisplayAlert("Error", message, "OK");
    }
}