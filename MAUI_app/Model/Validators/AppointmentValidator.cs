using FluentValidation;
using MAUI_app.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MAUI_app.Model.Validators;

public class AppointmentValidator : AbstractValidator<Appointment>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AppointmentValidator(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        RuleFor(x => x.PatientName)
            .NotEmpty().WithMessage("Patient name is required.")
            .MaximumLength(150).WithMessage("Patient name cannot exceed 150 characters.");

        RuleFor(x => x.DoctorId)
            .GreaterThan(0).WithMessage("A valid doctor must be selected.");

        RuleFor(x => x.AppointmentDate)
            .NotEmpty().WithMessage("Appointment date is required.")
            .Must(date => date >= DateTime.Now).WithMessage("The appointment date cannot be in the past.")
            .MustAsync(async (model, date, token) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var exists = await context.Appointments
                    .AnyAsync(a => a.AppointmentDate == date && a.Id != model.Id, token);

                return !exists;
            })
            .WithMessage("This appointment date is already scheduled in another record.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Appointment status is required.");

        RuleFor(x => x.ApplicationUserId)
            .GreaterThan(0).WithMessage("A valid application user is required.");

        RuleFor(x => x.MedicalNotes)
            .MaximumLength(500).WithMessage("Medical notes cannot exceed 500 characters.")
            .When(x => x.MedicalNotes != null);
    }
}