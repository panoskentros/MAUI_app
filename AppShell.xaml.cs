using Microsoft.Maui.Controls;

namespace MAUI_app;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }
    
    // The method for the custom footer
    private async void OnSignOutTapped(object sender, TappedEventArgs e)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync("//LoginPage");
    }
}