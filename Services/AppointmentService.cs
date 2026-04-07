using FluentValidation;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Services;


public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;
    private readonly IValidator<Appointment> _validator;
    public AppointmentService(AppDbContext context,IValidator<Appointment>  validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<List<Appointment>> GetUpcomingAppointmentsForPatientAsync(int userId)
    {
        var today = DateTime.Today;
        return await 
            _context.Set<Appointment>()
            .AsNoTracking()
            .Where(a => a.ApplicationUserId == userId && a.AppointmentDate >= today)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<int> GetTodaysAppointmentCountAsync()
    {
        var today = DateTime.Today;
        return await 
            _context.Set<Appointment>()
            .AsNoTracking()
            .CountAsync(a => a.AppointmentDate.Date == today);
    }

    public async Task<List<Appointment>> GetTodaysPatientsForDoctorAsync(int doctorId)
    {
        var rightNow = DateTime.Now; 

        return await 
            _context.Set<Appointment>()
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId && a.AppointmentDate >= rightNow)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<List<Appointment>> GetUpcomingAppointmentsForClinicAsync()
    {
        var today = DateTime.Today;
        return await 
            _context.Set<Appointment>()
            .AsNoTracking()
            .Where(a => a.AppointmentDate >= today)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }
    
    public async Task<IResult<Appointment>> CreateAppointmentAsync(Appointment appointment)
    {
        var validationResult = await _validator.ValidateAsync(appointment);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<Appointment>.Fail(errorMessages);
        }
        try
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return Result<Appointment>.Ok(appointment, "Appointment created successfully.");
        }
        catch (Exception ex)
        {
            return Result<Appointment>.Fail("A database error occurred: " + ex.Message);
        }
    }
}