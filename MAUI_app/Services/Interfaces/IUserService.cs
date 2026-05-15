using MAUI_app.Data;
using MAUI_app.Model;

namespace MAUI_app.Services.Interfaces;

public interface IUserService
{
    ApplicationUser? CurrentUser { get; }
    bool IsLoggedIn { get; }
    event EventHandler? UserChanged;
    Task<Result<ApplicationUser>> LoginAsync(string usernameOrEmail, string password);
    Task<List<ApplicationUser>> GetAllDoctorsAsync();
    Task<ApplicationUser?> GetDoctorByIdAsync(int doctorId);
    Task<ApplicationUser?> GetPatientByIdAsync(int doctorId);   
    Task<List<ApplicationUser>> GetAllPatientsAsync();
    Task<Result> RegisterAsync(ApplicationUser user);
    Task<Result> UpdateUserAsync(ApplicationUser updatedUser);
    void Logout();
}
