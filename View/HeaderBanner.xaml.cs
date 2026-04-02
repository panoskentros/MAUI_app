namespace MAUI_app.View;

public partial class HeaderBanner : ContentView
{
    public HeaderBanner()
    {
        InitializeComponent();
    }

    // We expose a public method so your page controllers can easily push the username here
    public void SetWelcomeMessage(string username)
    {
        if (!string.IsNullOrWhiteSpace(username))
        {
            WelcomeLabel.Text = $"Welcome, {username}!";
        }
    }
}