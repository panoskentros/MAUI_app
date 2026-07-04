using MAUI_app.Data;
using MAUI_app.Model;
using MAUI_app.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAUI_app.Services;

public class MedicationService : IMedicationService
{
    private readonly AppDbContext _context;

    public MedicationService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Medication>> GetMedicationsAsync(ApplicationUser currentUser)
    {
        if(currentUser.Role == UserRole.Doctor)
        {
            return await _context.Medications
                .Include(m => m.Patient)
                .Where(m => m.DoctorId == currentUser.Id)
                .ToListAsync();
        }
        else if (currentUser.Role == UserRole.Patient)
        {
            return await _context.Medications
                .Include(m => m.Doctor)
                .Where(m => m.ApplicationUserId == currentUser.Id)
                .ToListAsync();
        }
        else throw new Exception("Unsupported role");
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
    
    public async Task<bool> DeleteMedicationAsync(int medicationId)
    {
        try
        {
            var medToDelete = await _context.Medications.FindAsync(medicationId);
            
            if (medToDelete != null)
            {
                _context.Medications.Remove(medToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}