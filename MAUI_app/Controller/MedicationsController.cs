using MAUI_app.Model;
using MAUI_app.Data;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Controller;

public class MedicationsController
{
    private readonly AppDbContext _context;

    public MedicationsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Medication>> GetMedicationsAsync(ApplicationUser currentUser)
    {
        if (currentUser.Role == UserRole.Doctor || currentUser.Role == UserRole.Secretary)
        {
            return await _context.Medications
                .Include(m => m.Patient)
                .ToListAsync();
        }
        else if (currentUser.Role == UserRole.Patient)
        {
            return await _context.Medications
                .Include(m => m.Doctor)
                .Where(m => m.ApplicationUserId == currentUser.Id)
                .ToListAsync();
        }

        return new List<Medication>();
    }

    public async Task<bool> SaveMedicationAsync(Medication medication, ApplicationUser currentUser)
    {
        if (currentUser.Role != UserRole.Doctor)
        {
            return false;
        }

        if (medication.Id == 0)
        {
            _context.Medications.Add(medication);
        }
        else
        {
            _context.Medications.Update(medication);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}