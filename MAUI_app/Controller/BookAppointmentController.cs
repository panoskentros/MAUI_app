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

        // Αν είμαστε σε Edit Mode, προσυμπλήρωσε τα δεδομένα
        if (_appointmentToEdit.Id != 0)
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
            await _view.ShowAlertAsync("Error", "Please select a patient.");
            return;
        }
        if (selectedDoctor == null)
        {
            await _view.ShowAlertAsync("Error", "Please select a doctor.");
            return;
        }

        DateTime combined = date.Date + time;
        DateTime cleanDate = new DateTime(combined.Year, combined.Month, combined.Day, combined.Hour, combined.Minute, 0);

        Result<Appointment> serviceResult;

        _appointmentToEdit.AppointmentDate = cleanDate;
        _appointmentToEdit.MedicalNotes = notes;
        _appointmentToEdit.DoctorId = selectedDoctor.Id;

        if (_appointmentToEdit.Id != 0)
        {
            if (isStaff)
            {
                _appointmentToEdit.ApplicationUserId = selectedPatient.Id;
                _appointmentToEdit.PatientName = selectedPatient.UserName;
            }
    
            serviceResult = await _appointmentService.UpdateAppointmentAsync(_appointmentToEdit);
        }
        else
        {
            _appointmentToEdit.ApplicationUserId = isStaff ? selectedPatient.Id : currentUser.Id;
            _appointmentToEdit.PatientName = isStaff ? selectedPatient.UserName : currentUser.UserName;
            _appointmentToEdit.Status = "Scheduled";

            serviceResult = await _appointmentService.CreateAppointmentAsync(_appointmentToEdit);
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