using Microsoft.EntityFrameworkCore;
using Patient.Domain.Entities;
using Patient.Infrastructure.Data;
using SharedLibrary.Repository;

namespace Patient.Infrastructure.Repositories
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(PatientDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Set<MedicalRecord>()
                .Where(mr => mr.PatientId == patientId)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAndTypeAsync(int patientId, string recordType)
        {
            return await _context.Set<MedicalRecord>()
                .Where(mr => mr.PatientId == patientId && mr.RecordType == recordType)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Set<MedicalRecord>()
                .Where(mr => mr.DoctorId == doctorId)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetLatestByPatientIdAsync(int patientId)
        {
            return await _context.Set<MedicalRecord>()
                .Where(mr => mr.PatientId == patientId)
                .OrderByDescending(mr => mr.RecordDate)
                .FirstOrDefaultAsync();
        }
    }
}
