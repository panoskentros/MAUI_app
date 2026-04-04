using MAUI_app.Model;

namespace MAUI_app.Services.Interfaces;

public interface IAppointmentService
{
    Task<List<Appointment>> GetUpcomingAppointmentsForPatientAsync(int userId);
    Task<int> GetTodaysAppointmentCountAsync();
    Task<List<Appointment>> GetTodaysPatientsForDoctorAsync();
    Task<List<Appointment>> GetUpcomingAppointmentsForClinicAsync();
}