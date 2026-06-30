using MAUI_app.Data;
using MAUI_app.Model;

namespace MAUI_app.Services.Interfaces;

public interface IAppointmentService
{
    Task<List<Appointment>> GetUpcomingAppointmentsForPatientAsync(int userId);
    Task<int> GetTodaysAppointmentCountAsync();
    Task<List<Appointment>> GetTodaysPatientsForDoctorAsync(int doctorId);
    Task<List<Appointment>> GetUpcomingAppointmentsForClinicAsync();
    Task<Result<Appointment>> CreateAppointmentAsync(Appointment appointment);
    Task<Result<Appointment>> UpdateAppointmentAsync(Appointment appointment);
    Task<Result> CancelAppointmentAsync(int appointmentId);
    Task<List<Appointment>> GetPastAppointmentsForPatientAsync(int userId);
    Task<List<Appointment>> GetPastAppointmentsForDoctorAsync(int doctorId);
}