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
            var today = DateTime.Today;
            return await _context.Set<Appointment>()
                .AsNoTracking()
                .Where(a => a.ApplicationUserId == userId && a.AppointmentDate >= today)
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
            var today = DateTime.Today;
            return await _context.Set<Appointment>()
                .AsNoTracking()
                .CountAsync(a => a.AppointmentDate.Date == today);
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
            var rightNow = DateTime.Now; 

            return await _context.Set<Appointment>()
                .AsNoTracking()
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate >= rightNow)
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

    public async Task<List<Appointment>> GetUpcomingAppointmentsForClinicAsync()
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Set<Appointment>()
                .AsNoTracking()
                .Where(a => a.AppointmentDate >= today)
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
}