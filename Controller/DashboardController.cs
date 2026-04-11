using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.Data;

namespace MAUI_app.Controller;

public class DashboardController
{
    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;

    public DashboardController(IAppointmentService appointmentService, IUserService userService)
    {
        _appointmentService = appointmentService;
        _userService = userService;
    }

    public async Task<Result<DashboardData>> GetDashboardDataAsync()
    {
        var user = _userService.CurrentUser;
        if (user == null) return Result<DashboardData>.Fail("User not found.");

        var data = new DashboardData { Role = user.Role };

        switch (user.Role)
        {
            case UserRole.Patient:
                data.Appointments = await _appointmentService.GetUpcomingAppointmentsForPatientAsync(user.Id);
                break;
                
            case UserRole.Secretary:
                data.TodaysCount = await _appointmentService.GetTodaysAppointmentCountAsync();
                break;
                
            case UserRole.Doctor:
                data.Appointments = await _appointmentService.GetTodaysPatientsForDoctorAsync(user.Id);
                break;
        }

        return Result<DashboardData>.Ok(data, "Data loaded");
    }
}

public class DashboardData
{
    public List<Appointment> Appointments { get; set; } = new();
    public int TodaysCount { get; set; }
    public UserRole Role { get; set; }
}