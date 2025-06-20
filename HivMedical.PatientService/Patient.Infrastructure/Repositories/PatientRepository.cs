using Microsoft.EntityFrameworkCore;
using Patient.Infrastructure.Data;
using SharedLibrary.Repository;

namespace Patient.Infrastructure.Repositories
{
    public class PatientRepository : GenericRepository<Domain.Entities.Patient>, IPatientRepository
    {
        public PatientRepository(PatientDbContext context) : base(context)
        {
        }

        public async Task<Domain.Entities.Patient?> GetByPatientCodeAsync(string patientCode)
        {
            return await _context.Set<Domain.Entities.Patient>()
                .FirstOrDefaultAsync(p => p.PatientCode == patientCode);
        }

        public async Task<Domain.Entities.Patient?> GetByAuthUserIdAsync(int authUserId)
        {
            return await _context.Set<Domain.Entities.Patient>()
                .FirstOrDefaultAsync(p => p.AuthUserId == authUserId);
        }

        public async Task<Domain.Entities.Patient?> GetPatientWithRecordsAsync(int patientId)
        {
            return await _context.Set<Domain.Entities.Patient>()
                .Include(p => p.MedicalRecords)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<Domain.Entities.Patient?> GetPatientWithAppointmentsAsync(int patientId)
        {
            return await _context.Set<Domain.Entities.Patient>()
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<IEnumerable<Domain.Entities.Patient>> GetPatientsByStatusAsync(string status)
        {
            return await _context.Set<Domain.Entities.Patient>()
                .Where(p => p.HIVStatus == status)
                .ToListAsync();
        }

        public async Task<bool> PatientCodeExistsAsync(string patientCode)
        {
            return await _context.Set<Domain.Entities.Patient>()
                .AnyAsync(p => p.PatientCode == patientCode);
        }

        public async Task<bool> AuthUserIdExistsAsync(int authUserId)
        {
            return await _context.Set<Domain.Entities.Patient>()
                .AnyAsync(p => p.AuthUserId == authUserId);
        }
    }
}
