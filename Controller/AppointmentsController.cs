using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.Data;

namespace MAUI_app.Controller;

public class AppointmentsController
{
    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;

    public AppointmentsController(IAppointmentService appointmentService, IUserService userService)
    {
        _appointmentService = appointmentService;
        _userService = userService;
    }

    public async Task<Result<List<Appointment>>> GetDashboardDataAsync()
    {
        var user = _userService.CurrentUser;
        if (user == null) 
            return Result<List<Appointment>>.Fail("User not logged in");

        List<Appointment> appointments;

        if (user.Role == UserRole.Patient)
        {
            appointments = await _appointmentService.GetUpcomingAppointmentsForPatientAsync(user.Id);
            var allDoctors = await _userService.GetAllDoctorsAsync();
            
            foreach (var appt in appointments)
            {
                var doctor = allDoctors.FirstOrDefault(d => d.Id == appt.DoctorId);
                appt.DisplayName = doctor != null ? $"Dr. {doctor.UserName}" : "Unknown Doctor";
            }
        }
        else if (user.Role == UserRole.Secretary)
        {
            appointments = await _appointmentService.GetUpcomingAppointmentsForClinicAsync();
            foreach (var appt in appointments) 
                appt.DisplayName = appt.PatientName;
        }
        else 
        {
            appointments = await _appointmentService.GetTodaysPatientsForDoctorAsync(user.Id);
            foreach (var appt in appointments) 
                appt.DisplayName = appt.PatientName;
        }

        return Result<List<Appointment>>.Ok(appointments, "Success");
    }
}