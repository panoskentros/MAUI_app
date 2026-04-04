using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Services;


public class AppointmentService : IAppointmentService
{
    private readonly IRepository<Appointment> _appointmentRepo;

    public AppointmentService(IRepository<Appointment> appointmentRepo)
    {
        _appointmentRepo = appointmentRepo;
    }

    public async Task<List<Appointment>> GetUpcomingAppointmentsForPatientAsync(int userId)
    {
        var today = DateTime.Today;
        return await _appointmentRepo.GetQueryable().AsNoTracking()
            .Where(a => a.ApplicationUserId == userId && a.AppointmentDate >= today)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<int> GetTodaysAppointmentCountAsync()
    {
        var today = DateTime.Today;
        return await _appointmentRepo.GetQueryable().AsNoTracking()
            .CountAsync(a => a.AppointmentDate.Date == today);
    }

    public async Task<List<Appointment>> GetTodaysPatientsForDoctorAsync()
    {
        var today = DateTime.Today;
        return await _appointmentRepo.GetQueryable().AsNoTracking()
            .Where(a => a.AppointmentDate.Date == today)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<List<Appointment>> GetUpcomingAppointmentsForClinicAsync()
    {
        var today = DateTime.Today;
        return await _appointmentRepo.GetQueryable().AsNoTracking()
            .Where(a => a.AppointmentDate >= today)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }
}