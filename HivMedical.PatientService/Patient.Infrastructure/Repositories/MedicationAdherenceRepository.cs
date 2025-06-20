using Microsoft.EntityFrameworkCore;
using Patient.Domain.Entities;
using Patient.Infrastructure.Data;
using SharedLibrary.Repository;

namespace Patient.Infrastructure.Repositories
{
    public class MedicationAdherenceRepository : GenericRepository<MedicationAdherence>, IMedicationAdherenceRepository
    {
        public MedicationAdherenceRepository(PatientDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MedicationAdherence>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Set<MedicationAdherence>()
                .Include(ma => ma.Medication)
                .Include(ma => ma.PatientMedication)
                .Where(ma => ma.PatientId == patientId)
                .OrderByDescending(ma => ma.ScheduledDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationAdherence>> GetByPatientMedicationIdAsync(int patientMedicationId)
        {
            return await _context.Set<MedicationAdherence>()
                .Include(ma => ma.Medication)
                .Include(ma => ma.PatientMedication)
                .Where(ma => ma.PatientMedicationId == patientMedicationId)
                .OrderByDescending(ma => ma.ScheduledDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationAdherence>> GetByMedicationIdAsync(int medicationId)
        {
            return await _context.Set<MedicationAdherence>()
                .Include(ma => ma.Medication)
                .Include(ma => ma.PatientMedication)
                .Where(ma => ma.MedicationId == medicationId)
                .OrderByDescending(ma => ma.ScheduledDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationAdherence>> GetByDateRangeAsync(int patientId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<MedicationAdherence>()
                .Include(ma => ma.Medication)
                .Include(ma => ma.PatientMedication)
                .Where(ma => ma.PatientId == patientId &&
                           ma.ScheduledDateTime >= fromDate &&
                           ma.ScheduledDateTime <= toDate)
                .OrderByDescending(ma => ma.ScheduledDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationAdherence>> GetByStatusAsync(string status)
        {
            return await _context.Set<MedicationAdherence>()
                .Include(ma => ma.Medication)
                .Include(ma => ma.PatientMedication)
                .Where(ma => ma.Status == status)
                .OrderByDescending(ma => ma.ScheduledDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationAdherence>> GetMissedDosesAsync(int patientId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<MedicationAdherence>()
                .Include(ma => ma.Medication)
                .Include(ma => ma.PatientMedication)
                .Where(ma => ma.PatientId == patientId &&
                           ma.Status == "Missed" &&
                           ma.ScheduledDateTime >= fromDate &&
                           ma.ScheduledDateTime <= toDate)
                .OrderByDescending(ma => ma.ScheduledDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationAdherence>> GetFilteredAdherenceAsync(
            int? patientId = null,
            int? medicationId = null,
            int? patientMedicationId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Set<MedicationAdherence>()
                .Include(ma => ma.Medication)
                .Include(ma => ma.PatientMedication)
                .Include(ma => ma.Patient)
                .AsQueryable();

            if (patientId.HasValue)
                query = query.Where(ma => ma.PatientId == patientId.Value);

            if (medicationId.HasValue)
                query = query.Where(ma => ma.MedicationId == medicationId.Value);

            if (patientMedicationId.HasValue)
                query = query.Where(ma => ma.PatientMedicationId == patientMedicationId.Value);

            if (fromDate.HasValue)
                query = query.Where(ma => ma.ScheduledDateTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(ma => ma.ScheduledDateTime <= toDate.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(ma => ma.Status == status);

            return await query
                .OrderByDescending(ma => ma.ScheduledDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalFilteredCountAsync(
            int? patientId = null,
            int? medicationId = null,
            int? patientMedicationId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null)
        {
            var query = _context.Set<MedicationAdherence>().AsQueryable();

            if (patientId.HasValue)
                query = query.Where(ma => ma.PatientId == patientId.Value);

            if (medicationId.HasValue)
                query = query.Where(ma => ma.MedicationId == medicationId.Value);

            if (patientMedicationId.HasValue)
                query = query.Where(ma => ma.PatientMedicationId == patientMedicationId.Value);

            if (fromDate.HasValue)
                query = query.Where(ma => ma.ScheduledDateTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(ma => ma.ScheduledDateTime <= toDate.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(ma => ma.Status == status);

            return await query.CountAsync();
        }

        public async Task<MedicationAdherence?> GetByScheduledDateTimeAsync(int patientMedicationId, DateTime scheduledDateTime)
        {
            return await _context.Set<MedicationAdherence>()
                .Include(ma => ma.Medication)
                .Include(ma => ma.PatientMedication)
                .FirstOrDefaultAsync(ma => ma.PatientMedicationId == patientMedicationId &&
                                         ma.ScheduledDateTime == scheduledDateTime);
        }

        public async Task<int> GetTotalScheduledDosesAsync(int patientId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<MedicationAdherence>()
                .Where(ma => ma.PatientId == patientId &&
                           ma.ScheduledDateTime >= fromDate &&
                           ma.ScheduledDateTime <= toDate)
                .CountAsync();
        }

        public async Task<int> GetTakenDosesCountAsync(int patientId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<MedicationAdherence>()
                .Where(ma => ma.PatientId == patientId &&
                           ma.Status == "Taken" &&
                           ma.ScheduledDateTime >= fromDate &&
                           ma.ScheduledDateTime <= toDate)
                .CountAsync();
        }

        public async Task<int> GetMissedDosesCountAsync(int patientId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<MedicationAdherence>()
                .Where(ma => ma.PatientId == patientId &&
                           ma.Status == "Missed" &&
                           ma.ScheduledDateTime >= fromDate &&
                           ma.ScheduledDateTime <= toDate)
                .CountAsync();
        }

        public async Task<decimal> CalculateAdherencePercentageAsync(int patientId, DateTime fromDate, DateTime toDate)
        {
            var totalScheduled = await GetTotalScheduledDosesAsync(patientId, fromDate, toDate);
            if (totalScheduled == 0) return 0;

            var takenCount = await GetTakenDosesCountAsync(patientId, fromDate, toDate);
            return Math.Round((decimal)takenCount / totalScheduled * 100, 2);
        }

        public async Task<IEnumerable<string>> GetCommonMissedReasonsAsync(int patientId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<MedicationAdherence>()
                .Where(ma => ma.PatientId == patientId &&
                           ma.Status == "Missed" &&
                           ma.ScheduledDateTime >= fromDate &&
                           ma.ScheduledDateTime <= toDate &&
                           !string.IsNullOrEmpty(ma.Reason))
                .GroupBy(ma => ma.Reason)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key!)
                .Take(5)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetReportedSideEffectsAsync(int patientId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<MedicationAdherence>()
                .Where(ma => ma.PatientId == patientId &&
                           ma.ScheduledDateTime >= fromDate &&
                           ma.ScheduledDateTime <= toDate &&
                           !string.IsNullOrEmpty(ma.SideEffectsReported))
                .Select(ma => ma.SideEffectsReported!)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationAdherence>> GetRecentAdherenceAsync(int patientId, int days = 7)
        {
            var fromDate = DateTime.Now.AddDays(-days);
            return await _context.Set<MedicationAdherence>()
                .Include(ma => ma.Medication)
                .Include(ma => ma.PatientMedication)
                .Where(ma => ma.PatientId == patientId &&
                           ma.ScheduledDateTime >= fromDate)
                .OrderByDescending(ma => ma.ScheduledDateTime)
                .ToListAsync();
        }
    }
}
