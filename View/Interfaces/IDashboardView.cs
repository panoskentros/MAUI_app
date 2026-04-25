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
    
    Task NavigateToBookAppointmentAsync();
    Task NavigateToAppointmentsAsync();
    Task NavigateToMedicationsAsync();
    Task NavigateToSettingsAsync();
}