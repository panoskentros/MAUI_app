using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.interfaces;

namespace MAUI_app.Controller;

public class BookAppointmentController
{
    private readonly IBookAppointmentView _view;
    private readonly IAppointmentService _appointmentService;
    private readonly IUserService _userService;
    
    private Appointment _appointmentToEdit;

    public BookAppointmentController(
        IBookAppointmentView view, 
        IAppointmentService appointmentService, 
        IUserService userService)
    {
        _view = view;
        _appointmentService = appointmentService;
        _userService = userService;
    }

    public void SetAppointmentToEdit(Appointment appt)
    {
        _appointmentToEdit = appt;
    }

    public async Task OnViewAppearing()
    {
        var user = _userService.CurrentUser;
        if (user == null) return;

        bool isStaff = user.Role == UserRole.Secretary || user.Role == UserRole.Doctor;
        _view.ShowPatientSelection(isStaff);

        var doctors = await _userService.GetAllDoctorsAsync();
        _view.SetDoctors(doctors);

        List<ApplicationUser> patients = null;
        if (isStaff)
        {
            patients = await _userService.GetAllPatientsAsync();
            _view.SetPatients(patients);
        }
        
        if (_appointmentToEdit != null && _appointmentToEdit.Id != 0)
        {
            _view.PrefillData(_appointmentToEdit, doctors, patients);
            _view.SetSubmitButtonText("Update Appointment");
        }
        else
        {
            _view.SetSubmitButtonText("Confirm Appointment");
        }
    }

    public async Task SaveAppointment(ApplicationUser selectedDoctor, ApplicationUser selectedPatient, DateTime date, TimeSpan time, string notes)
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
            await _view.ShowAlertAsync("Error", "Please select a patient from the list before confirming.");
            return;
        }
        
        if (selectedDoctor == null)
        {
            await _view.ShowAlertAsync("Error", "Please select a doctor from the list before confirming.");
            return;
        }

        string safeNotes = notes ?? string.Empty;

        DateTime combined = date.Date + time;
        DateTime cleanDate = new DateTime(combined.Year, combined.Month, combined.Day, combined.Hour, combined.Minute, 0);

        Result<Appointment> serviceResult;

        if (_appointmentToEdit != null && _appointmentToEdit.Id != 0)
        {
            _appointmentToEdit.AppointmentDate = cleanDate;
            _appointmentToEdit.MedicalNotes = safeNotes;
            _appointmentToEdit.DoctorId = selectedDoctor.Id;
            
            if (isStaff)
            {
                _appointmentToEdit.ApplicationUserId = selectedPatient.Id;
            }
    
            serviceResult = await _appointmentService.UpdateAppointmentAsync(_appointmentToEdit);
        }
        else
        {
            int patientId = isStaff ? selectedPatient.Id : currentUser.Id;

            var newAppointment = new Appointment
            {
                ApplicationUserId = patientId,
                DoctorId = selectedDoctor.Id,
                AppointmentDate = cleanDate,
                MedicalNotes = safeNotes,
                Status = "Scheduled"
            };

            serviceResult = await _appointmentService.CreateAppointmentAsync(newAppointment);
        }

        if (serviceResult.Success)
        {
            await _view.ShowAlertAsync("Success", serviceResult.Message);
            await _view.NavigateBackAsync();
        }
        else
        {
            await _view.ShowAlertAsync("Error", serviceResult.Message);
        }
    }
}