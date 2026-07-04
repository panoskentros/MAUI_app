using FluentValidation;
using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MAUI_app.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;
    private readonly IValidator<Appointment> _validator;

    public AppointmentService(AppDbContext context, IValidator<Appointment> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<List<Appointment>> GetUpcomingAppointmentsForPatientAsync(int userId)
    {
        try
        {
            var datetimeNow = DateTime.UtcNow;
            return await _context.Set<Appointment>()
                .AsNoTracking()
                .Include(a => a.ApplicationUser)
                .Where(a => a.ApplicationUserId == userId && a.AppointmentDate >= datetimeNow)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }
        catch (InvalidOperationException ex) when (ex.InnerException is NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
        catch (NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
    }

    public async Task<int> GetTodaysAppointmentCountAsync()
    {
        try
        {
            var datetimeNow = DateTime.UtcNow;
            var tomorrow = datetimeNow.Date.AddDays(1);

            return await _context.Set<Appointment>()
                .AsNoTracking()
                .CountAsync(a => a.AppointmentDate >= datetimeNow && a.AppointmentDate < tomorrow);
        }
        catch (InvalidOperationException ex) when (ex.InnerException is NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
        catch (NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
    }

    public async Task<List<Appointment>> GetTodaysPatientsForDoctorAsync(int doctorId)
    {
        try
        {
            var datetimeNow = DateTime.UtcNow;
            var tomorrow = datetimeNow.Date.AddDays(1);

            return await _context.Set<Appointment>()
                .AsNoTracking()
                .Include(a => a.ApplicationUser)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate >= datetimeNow && a.AppointmentDate < tomorrow)
                .ToListAsync();
        }
        catch (InvalidOperationException ex) when (ex.InnerException is NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
        catch (NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
    }

    public async Task<List<Appointment>> GetUpcomingAppointmentsForClinicAsync()
    {
        try
        {
            var datetimeNow = DateTime.UtcNow;
            return await _context.Set<Appointment>()
                .AsNoTracking()
                .Include(a => a.ApplicationUser)
                .Where(a => a.AppointmentDate >= datetimeNow)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }
        catch (InvalidOperationException ex) when (ex.InnerException is NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
        catch (NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
    }
    
    public async Task<Result<Appointment>> CreateAppointmentAsync(Appointment appointment)
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
        catch (InvalidOperationException ex) when (ex.InnerException is NpgsqlException)
        {
            return Result<Appointment>.Fail("The database is currently offline. Please try again later.");
        }
        catch (NpgsqlException)
        {
            return Result<Appointment>.Fail("The database is currently offline. Please try again later.");
        }
        catch (DbUpdateException)
        {
            return Result<Appointment>.Fail("There is already an appointment with the given date...Please refresh the view.");
        }
        catch (Exception ex)
        {
            return Result<Appointment>.Fail("A database error occurred: " + ex.Message);
        }
    }
    
    public async Task<Result<Appointment>> UpdateAppointmentAsync(Appointment appointment)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(appointment);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<Appointment>.Fail(errorMessages);
            }
            
            var existingTracking = _context.Appointments.Local.FirstOrDefault(a => a.Id == appointment.Id);
            if (existingTracking != null)
            {
                _context.Entry(existingTracking).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            _context.Entry(appointment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
        
            return Result<Appointment>.Ok(appointment, "Appointment updated successfully.");
        }
        catch (DbUpdateException)
        {
            return Result<Appointment>.Fail("There is already an appointment with the given date.");
        }
        catch (Exception ex)
        {
            return Result<Appointment>.Fail("A database error occurred: " + ex.Message);
        }
    }

    public async Task<Result> CancelAppointmentAsync(int appointmentId)
    {
        try
        {
            int affectedRows = await _context.Appointments
                .Where(a => a.Id == appointmentId)
                .ExecuteDeleteAsync();

            if (affectedRows == 0)
                return Result.Fail("Appointment not found or already cancelled.");

            return Result.Ok("Appointment cancelled successfully.");
        }
        catch (InvalidOperationException ex) when (ex.InnerException is NpgsqlException)
        {
            return Result.Fail("The database is currently offline. Please try again later.");
        }
        catch (NpgsqlException)
        {
            return Result.Fail("The database is currently offline. Please try again later.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Database error: {ex.Message}");
        }
    }
    
    public async Task<List<Appointment>> GetPastAppointmentsForPatientAsync(int userId)
    {
        try
        {
            var datetimeNow = DateTime.UtcNow;
            return await _context.Set<Appointment>()
                .AsNoTracking()
                .Include(a => a.ApplicationUser)
                .Where(a => a.ApplicationUserId == userId && a.AppointmentDate < datetimeNow)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }
        catch (InvalidOperationException ex) when (ex.InnerException is NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
        catch (NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
    }

    public async Task<List<Appointment>> GetPastAppointmentsForDoctorAsync(int doctorId)
    {
        try
        {
            var datetimeNow = DateTime.UtcNow;
            return await _context.Set<Appointment>()
                .AsNoTracking()
                .Include(a => a.ApplicationUser)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate < datetimeNow)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }
        catch (InvalidOperationException ex) when (ex.InnerException is NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
        catch (NpgsqlException)
        {
            throw new Exception("The database is currently offline. Please try again later.");
        }
    }
}