using FluentValidation;

namespace MAUI_app.Model.Validators;

public class MedicationValidator : AbstractValidator<Medication>
{
    public MedicationValidator()
    {
        RuleFor(x => x.MedicationName)
            .NotEmpty().WithMessage("Medication name is required.")
            .MaximumLength(150).WithMessage("Medication name cannot exceed 150 characters.");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Instructions are required.")
            .MaximumLength(500).WithMessage("Instructions cannot exceed 500 characters.");

        RuleFor(x => x.DoctorId)
            .GreaterThan(0).WithMessage("A doctor must be selected.");

        RuleFor(x => x.ApplicationUserId)
            .GreaterThan(0).WithMessage("A patient must be selected.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(date => date.Date >= DateTime.Today).WithMessage("The start date cannot be in the past.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("The end date cannot be before the start date.");
    }
}