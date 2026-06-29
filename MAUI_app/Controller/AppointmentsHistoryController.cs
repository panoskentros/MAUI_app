using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.Controller;

public class AppointmentsHistoryController
{
    private readonly IAppointmentsHistoryView _view;
    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;

    public AppointmentsHistoryController(
        IAppointmentsHistoryView view, 
        IAppointmentService appointmentService, 
        IUserService userService)
    {
        _view = view;
        _appointmentService = appointmentService;
        _userService = userService;
    }

    public async Task LoadHistoryAsync()
    {
        var user = _userService.CurrentUser;
        if (user == null) return;

        IEnumerable<Appointment> rawAppointments;

        if (user.Role == UserRole.Patient)
        {
            var patientAppts = await _appointmentService.GetPastAppointmentsForPatientAsync(user.Id);
            rawAppointments = patientAppts;
            
            var allDoctors = await _userService.GetAllDoctorsAsync();
            
            foreach (var appt in rawAppointments)
            {
                var doctor = allDoctors.FirstOrDefault(d => d.Id == appt.DoctorId);
                appt.DisplayName = doctor != null ? $"Dr. {doctor.UserName}" : "Unknown Doctor";
            }
        }
        else
        {
            var doctorAppts = await _appointmentService.GetPastAppointmentsForDoctorAsync(user.Id);
            rawAppointments = doctorAppts;
            
            foreach (var appt in rawAppointments)
            {
                appt.DisplayName = appt.PatientName;
            }
        }

        var groupedData = rawAppointments
            .Where(a => a.AppointmentDate < DateTime.Now)
            .OrderByDescending(a => a.AppointmentDate)
            .GroupBy(a => a.AppointmentDate.Date.ToString("MMMM dd, yyyy"))
            .ToList();

        _view.SetHistoryAppointments(groupedData);
    }
}