using System.ComponentModel;
using System.Runtime.CompilerServices;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;

namespace MAUI_app.Controller;

public class BaseController : INotifyPropertyChanged 
{
    protected readonly IUserService _userService;
    
    private string _bannerTitle = string.Empty;
    public string BannerTitle
    {
        get => _bannerTitle;
        set { _bannerTitle = value; OnPropertyChanged(); }
    }

    private string _bannerWelcomeMessage = string.Empty;
    public string BannerWelcomeMessage
    {
        get => _bannerWelcomeMessage;
        set { _bannerWelcomeMessage = value; OnPropertyChanged(); }
    }

    private bool _isPatientViewVisible;
    public bool IsPatientViewVisible
    {
        get => _isPatientViewVisible;
        set { _isPatientViewVisible = value; OnPropertyChanged(); }
    }

    private bool _isSecretaryViewVisible;
    public bool IsSecretaryViewVisible
    {
        get => _isSecretaryViewVisible;
        set { _isSecretaryViewVisible = value; OnPropertyChanged(); }
    }

    private bool _isDoctorViewVisible;
    public bool IsDoctorViewVisible
    {
        get => _isDoctorViewVisible;
        set { _isDoctorViewVisible = value; OnPropertyChanged(); }
    }
    
    public BaseController(IUserService userService)
    {
        _userService = userService;
    }
    
    public void SetupBanner(string title, bool showUserName = true, string customMessage = "")
    {
        var user = _userService.CurrentUser;
        if (user == null) return;
        
        IsPatientViewVisible = user.Role == UserRole.Patient;
        IsSecretaryViewVisible = user.Role == UserRole.Secretary;
        IsDoctorViewVisible = user.Role == UserRole.Doctor;

        BannerTitle = title;
        
        if (showUserName)
        {
            string name = IsDoctorViewVisible ? "Dr. " + user.UserName : user.UserName;
            BannerWelcomeMessage = $"Welcome, {name}!";
        }
        else
        {
            BannerWelcomeMessage = customMessage;
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}