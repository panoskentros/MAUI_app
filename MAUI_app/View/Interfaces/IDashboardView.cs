using MAUI_app.Model;

namespace MAUI_app.View.interfaces;

public interface IDashboardView
{
    void ShowPatientView(bool isVisible);
    void ShowSecretaryView(bool isVisible);
    void ShowDoctorView(bool isVisible);
    
    void SetPatientNextAppointment(string dateText, string detailsText);
    void SetPatientMoreAppointmentsButton(bool isVisible, string buttonText = "");
    
    void SetSecretaryAppointmentsToday(string countText);
    
    void SetDoctorNextPatient(string timeAndNameText, string detailsText);
    void SetDoctorMorePatientsButton(bool isVisible, string buttonText = "");
    
    void SetPatientRescheduleButton(bool isVisible);
    Task NavigateToBookAppointmentAsync(Appointment appt = null!);
    Task NavigateToAppointmentsAsync();
    Task NavigateToMedicationsAsync();
    Task NavigateToAddMedicationAsync(Medication med = null);
}