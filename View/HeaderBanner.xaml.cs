namespace MAUI_app.View;

public partial class HeaderBanner : ContentView
{
    public HeaderBanner()
    {
        InitializeComponent();
    }
    
    public void SetTitle(string newTitle)
    {
        BannerTitleLabel.Text = newTitle;
    }
    
    public void SetWelcomeMessage(string username)
    {
        if (!string.IsNullOrWhiteSpace(username))
        {
            WelcomeLabel.Text = $"Welcome, {username}!";
        }
    }
}