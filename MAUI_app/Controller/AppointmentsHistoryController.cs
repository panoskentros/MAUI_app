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
        else if (user.Role == UserRole.Secretary)
        {
            var allPastAppts = new List<Appointment>();
            var doctors = await _userService.GetAllDoctorsAsync();
    
            foreach (var doctor in doctors)
            {
                var doctorAppts = await _appointmentService.GetPastAppointmentsForDoctorAsync(doctor.Id);
        
                foreach (var appt in doctorAppts)
                {
                    appt.DisplayName = $"Dr. {doctor.UserName} - Patient: {appt.ApplicationUser.UserName}";
                }
        
                allPastAppts.AddRange(doctorAppts);
            }

            rawAppointments = allPastAppts
                .OrderBy(a => doctors.FirstOrDefault(d => d.Id == a.DoctorId)?.UserName)
                .ThenByDescending(a => a.AppointmentDate)
                .ToList();
        }
        else
        {
            var doctorAppts = await _appointmentService.GetPastAppointmentsForDoctorAsync(user.Id);
            rawAppointments = doctorAppts;
            
            foreach (var appt in rawAppointments)
            {
                appt.DisplayName = appt.ApplicationUser.UserName;
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