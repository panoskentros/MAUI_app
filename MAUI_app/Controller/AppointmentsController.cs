using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;
using MAUI_app.Data;

namespace MAUI_app.Controller;

public class AppointmentsController
{
    private readonly IAppointmentsView _view;
    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;
    public ObservableCollection<Appointment> appointments;

    public AppointmentsController(
        IAppointmentsView view, 
        IAppointmentService appointmentService, 
        IUserService userService)
    {
        _view = view;
        _appointmentService = appointmentService;
        _userService = userService;
    }

    public async Task InitializeDataAsync()
    {
        var user = _userService.CurrentUser;
        if (user == null)
        {
            await _view.ShowMessageAsync("User not logged in", true);
            return;
        }

        List<Appointment> appointmentsList; 

        if (user.Role == UserRole.Patient)
        {
            appointmentsList = await _appointmentService.GetUpcomingAppointmentsForPatientAsync(user.Id);
            var allDoctors = await _userService.GetAllDoctorsAsync();
        
            foreach (var appt in appointmentsList)
            {
                var doctor = allDoctors.FirstOrDefault(d => d.Id == appt.DoctorId);
                appt.DisplayName = doctor != null ? $"Dr. {doctor.UserName}" : "Unknown Doctor";
            }
        }
        else if (user.Role == UserRole.Secretary)
        {
            appointmentsList = await _appointmentService.GetUpcomingAppointmentsForClinicAsync();
            foreach (var appt in appointmentsList) 
            {
                appt.DisplayName = appt.PatientName;
            }
        }
        else 
        {
            appointmentsList = await _appointmentService.GetTodaysPatientsForDoctorAsync(user.Id);
            foreach (var appt in appointmentsList) 
            {
                appt.DisplayName = appt.PatientName;
            }
        }

        appointments = new ObservableCollection<Appointment>(appointmentsList);
        _view.SetAppointments(appointments);
    }

    public async Task CancelAppointmentAsync(Appointment appointment)
    {
        var result = await _appointmentService.CancelAppointmentAsync(appointment.Id);
        await _view.ShowMessageAsync(result.Message);
        
        if (result.Success)
        {
            appointments?.Remove(appointment);
        }
    }
}