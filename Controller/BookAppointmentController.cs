using System.Collections.Generic;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.Data;

namespace MAUI_app.Controller;

public class BookAppointmentController
{
    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;

    public BookAppointmentController(IAppointmentService appointmentService, IUserService userService)
    {
        _appointmentService = appointmentService;
        _userService = userService;
    }

    public async Task<List<ApplicationUser>> GetDoctorsAsync()
    {
        return await _userService.GetAllDoctorsAsync();
    }

    public async Task<List<ApplicationUser>> GetPatientsAsync()
    {
        var user = _userService.CurrentUser;
        if (user != null && (user.Role == UserRole.Secretary || user.Role == UserRole.Doctor))
        {
            return await _userService.GetAllPatientsAsync();
        }
        return new List<ApplicationUser>();
    }

    public async Task<Result> SaveAppointmentAsync(
        ApplicationUser selectedDoctor, 
        ApplicationUser selectedPatient, 
        DateTime date, 
        TimeSpan time, 
        string notes)
    {
        var currentUser = _userService.CurrentUser;
        if (currentUser == null) return Result.Fail("User not authenticated.");

        bool isStaff = currentUser.Role == UserRole.Secretary || currentUser.Role == UserRole.Doctor;
        if (isStaff) return Result.Fail("Please select a patient.");

        DateTime combined = date.Date + time;
        DateTime cleanDate = new DateTime(combined.Year, combined.Month, combined.Day, combined.Hour, combined.Minute, 0);

        int patientId = isStaff ? selectedPatient.Id : currentUser.Id;
        string patientName = isStaff ? selectedPatient.UserName : currentUser.UserName;

        var newAppointment = new Appointment
        {
            ApplicationUserId = patientId,
            DoctorId = selectedDoctor.Id,
            PatientName = patientName,
            AppointmentDate = cleanDate,
            MedicalNotes = notes,
            Status = "Scheduled"
        };

        var serviceResult = await _appointmentService.CreateAppointmentAsync(newAppointment);

        if (serviceResult.Success)
            return Result.Ok("Appointment saved successfully.");

        return Result.Fail(serviceResult.Message);
    }
}