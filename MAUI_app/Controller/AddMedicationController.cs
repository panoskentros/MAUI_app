using MAUI_app.Model;
using MAUI_app.Model.Validators;
using MAUI_app.Services.Interfaces;
using MAUI_app.View.Interfaces;

namespace MAUI_app.Controller;

public class AddMedicationController
{
    private readonly IAddMedicationView _view;
    private readonly IMedicationService _medicationService;
    private readonly IUserService _userService;
    private Medication? _medicationToEdit;
    private List<ApplicationUser> _patients = new();

    public AddMedicationController(IAddMedicationView view, IMedicationService medicationService, IUserService userService)
    {
        _view = view;
        _medicationService = medicationService;
        _userService = userService;
    }

    public void SetMedicationToEdit(Medication medication)
    {
        _medicationToEdit = medication;
    }

    public async Task OnViewAppearing()
    {
        try
        {
            _patients = await _userService.GetAllPatientsAsync();
            _view.SetPatients(_patients);

            if (_medicationToEdit != null)
            {
                if (_medicationToEdit.ApplicationUserId != 0)
                {
                    _view.PrefillData(_medicationToEdit, _patients);
                }
                
                _view.SetSubmitButtonText("Update Prescription");
            }
            else
            {
                _view.SetSubmitButtonText("Save Prescription");
            }
        }
        catch (Exception ex)
        {
            await _view.ShowAlertAsync("Error", "Failed to load patients: " + ex.Message);
        }
    }

    public async Task SaveMedication(ApplicationUser? selectedPatient, string name, string instructions, DateTime startDate, DateTime endDate)
    {
        if (selectedPatient == null)
        {
            await _view.ShowAlertAsync("Validation Error", "Please select a patient.");
            return;
        }

        var doctor = _userService.CurrentUser;
        if (doctor == null || doctor.Role != UserRole.Doctor)
        {
            await _view.ShowAlertAsync("Unauthorized", "Only doctors can prescribe medication.");
            return;
        }

        var medication = _medicationToEdit ?? new Medication();
        
        medication.MedicationName = name ?? string.Empty;
        medication.Instructions = instructions ?? string.Empty;
        medication.StartDate = startDate;
        medication.EndDate = endDate;
        medication.ApplicationUserId = selectedPatient.Id;
        medication.DoctorId = doctor.Id;

        var validator = new MedicationValidator();
        var validationResult = validator.Validate(medication);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
            await _view.ShowAlertAsync("Validation Error", errors);
            return;
        }

        try
        {
            bool success = await _medicationService.SaveMedicationAsync(medication, doctor);

            if (success)
            {
                await _view.ShowAlertAsync("Success", "Prescription saved successfully.");
                await _view.NavigateBackAsync();
            }
            else
            {
                await _view.ShowAlertAsync("Error", "Failed to save the prescription.");
            }
        }
        catch (Exception ex)
        {
            await _view.ShowAlertAsync("Error", "An error occurred: " + ex.Message);
        }
    }
}