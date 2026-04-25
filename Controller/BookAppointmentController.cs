using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.Controller;

public class BookAppointmentController
{
    private readonly IBookAppointmentView _view;
    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;

    public BookAppointmentController(
        IBookAppointmentView view, 
        IAppointmentService appointmentService, 
        IUserService userService)
    {
        _view = view;
        _appointmentService = appointmentService;
        _userService = userService;
    }

    public async Task OnViewAppearing()
    {
        var user = _userService.CurrentUser;
        if (user == null) return;

        bool isStaff = user.Role == UserRole.Secretary || user.Role == UserRole.Doctor;
        
        _view.ShowPatientSelection(isStaff);

        var doctors = await _userService.GetAllDoctorsAsync();
        _view.SetDoctors(doctors);

        if (isStaff)
        {
            var patients = await _userService.GetAllPatientsAsync();
            _view.SetPatients(patients);
        }
    }

    public async Task SaveAppointment(
        ApplicationUser selectedDoctor, 
        ApplicationUser selectedPatient, 
        DateTime date, 
        TimeSpan time, 
        string notes)
    {
        var currentUser = _userService.CurrentUser;
        if (currentUser == null) 
        {
            await _view.ShowAlertAsync("Error", "User not authenticated.");
            return;
        }

        bool isStaff = currentUser.Role == UserRole.Secretary || currentUser.Role == UserRole.Doctor;
        
        if (isStaff && selectedPatient == null) 
        {
            await _view.ShowAlertAsync("Error", "Please select a patient.");
            return;
        }

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
        {
            await _view.ShowAlertAsync("Success", "Appointment saved successfully.");
            await _view.NavigateBackAsync();
        }
        else
        {
            await _view.ShowAlertAsync("Error", serviceResult.Message);
        }
    }
}