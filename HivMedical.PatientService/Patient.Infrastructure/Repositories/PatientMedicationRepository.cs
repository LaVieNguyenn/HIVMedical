using Microsoft.EntityFrameworkCore;
using Patient.Domain.Entities;
using Patient.Infrastructure.Data;
using SharedLibrary.Repository;

namespace Patient.Infrastructure.Repositories
{
    public class PatientMedicationRepository : GenericRepository<PatientMedication>, IPatientMedicationRepository
    {
        public PatientMedicationRepository(PatientDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PatientMedication>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .Where(pm => pm.PatientId == patientId)
                .OrderByDescending(pm => pm.PrescribedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientMedication>> GetByMedicationIdAsync(int medicationId)
        {
            return await _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .Where(pm => pm.MedicationId == medicationId)
                .OrderByDescending(pm => pm.PrescribedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientMedication>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .Where(pm => pm.PrescribedByDoctorId == doctorId)
                .OrderByDescending(pm => pm.PrescribedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientMedication>> GetActiveByPatientIdAsync(int patientId)
        {
            return await _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .Where(pm => pm.PatientId == patientId && pm.Status == "Active")
                .OrderBy(pm => pm.Medication.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientMedication>> GetCurrentMedicationsByPatientIdAsync(int patientId)
        {
            return await _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .Where(pm => pm.PatientId == patientId && pm.IsCurrentlyTaking)
                .OrderBy(pm => pm.Medication.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientMedication>> GetByStatusAsync(string status)
        {
            return await _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .Where(pm => pm.Status == status)
                .OrderByDescending(pm => pm.PrescribedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientMedication>> GetUpcomingRefillsAsync(int days = 7)
        {
            var cutoffDate = DateTime.Now.AddDays(days);
            return await _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .Where(pm => pm.IsCurrentlyTaking && 
                           pm.NextRefillDue.HasValue && 
                           pm.NextRefillDue.Value <= cutoffDate)
                .OrderBy(pm => pm.NextRefillDue)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientMedication>> GetFilteredPatientMedicationsAsync(
            int? patientId = null,
            int? medicationId = null,
            int? prescribedByDoctorId = null,
            string? status = null,
            bool? isCurrentlyTaking = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            string? category = null,
            string? medicationType = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .AsQueryable();

            if (patientId.HasValue)
                query = query.Where(pm => pm.PatientId == patientId.Value);

            if (medicationId.HasValue)
                query = query.Where(pm => pm.MedicationId == medicationId.Value);

            if (prescribedByDoctorId.HasValue)
                query = query.Where(pm => pm.PrescribedByDoctorId == prescribedByDoctorId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(pm => pm.Status == status);

            if (isCurrentlyTaking.HasValue)
                query = query.Where(pm => pm.IsCurrentlyTaking == isCurrentlyTaking.Value);

            if (startDateFrom.HasValue)
                query = query.Where(pm => pm.StartDate >= startDateFrom.Value.Date);

            if (startDateTo.HasValue)
                query = query.Where(pm => pm.StartDate <= startDateTo.Value.Date);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(pm => pm.Medication.Category == category);

            if (!string.IsNullOrEmpty(medicationType))
                query = query.Where(pm => pm.Medication.MedicationType == medicationType);

            return await query
                .OrderByDescending(pm => pm.PrescribedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalFilteredCountAsync(
            int? patientId = null,
            int? medicationId = null,
            int? prescribedByDoctorId = null,
            string? status = null,
            bool? isCurrentlyTaking = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            string? category = null,
            string? medicationType = null)
        {
            var query = _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .AsQueryable();

            if (patientId.HasValue)
                query = query.Where(pm => pm.PatientId == patientId.Value);

            if (medicationId.HasValue)
                query = query.Where(pm => pm.MedicationId == medicationId.Value);

            if (prescribedByDoctorId.HasValue)
                query = query.Where(pm => pm.PrescribedByDoctorId == prescribedByDoctorId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(pm => pm.Status == status);

            if (isCurrentlyTaking.HasValue)
                query = query.Where(pm => pm.IsCurrentlyTaking == isCurrentlyTaking.Value);

            if (startDateFrom.HasValue)
                query = query.Where(pm => pm.StartDate >= startDateFrom.Value.Date);

            if (startDateTo.HasValue)
                query = query.Where(pm => pm.StartDate <= startDateTo.Value.Date);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(pm => pm.Medication.Category == category);

            if (!string.IsNullOrEmpty(medicationType))
                query = query.Where(pm => pm.Medication.MedicationType == medicationType);

            return await query.CountAsync();
        }

        public async Task<PatientMedication?> GetWithMedicationAsync(int id)
        {
            return await _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .FirstOrDefaultAsync(pm => pm.Id == id);
        }

        public async Task<IEnumerable<PatientMedication>> GetWithMedicationByPatientIdAsync(int patientId)
        {
            return await _context.Set<PatientMedication>()
                .Include(pm => pm.Medication)
                .Include(pm => pm.Patient)
                .Where(pm => pm.PatientId == patientId)
                .OrderByDescending(pm => pm.PrescribedDate)
                .ToListAsync();
        }

        public async Task<bool> HasActiveConflictingMedicationAsync(int patientId, int medicationId, int? excludeId = null)
        {
            var query = _context.Set<PatientMedication>()
                .Where(pm => pm.PatientId == patientId && 
                           pm.MedicationId == medicationId && 
                           pm.IsCurrentlyTaking);

            if (excludeId.HasValue)
                query = query.Where(pm => pm.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<decimal> CalculateAdherencePercentageAsync(int patientMedicationId, DateTime fromDate, DateTime toDate)
        {
            var totalScheduled = await _context.Set<MedicationAdherence>()
                .Where(ma => ma.PatientMedicationId == patientMedicationId &&
                           ma.ScheduledDateTime >= fromDate &&
                           ma.ScheduledDateTime <= toDate)
                .CountAsync();

            if (totalScheduled == 0) return 0;

            var takenCount = await _context.Set<MedicationAdherence>()
                .Where(ma => ma.PatientMedicationId == patientMedicationId &&
                           ma.ScheduledDateTime >= fromDate &&
                           ma.ScheduledDateTime <= toDate &&
                           ma.Status == "Taken")
                .CountAsync();

            return Math.Round((decimal)takenCount / totalScheduled * 100, 2);
        }

        public async Task UpdateAdherenceStatsAsync(int patientMedicationId)
        {
            var patientMedication = await _context.Set<PatientMedication>()
                .FirstOrDefaultAsync(pm => pm.Id == patientMedicationId);

            if (patientMedication == null) return;

            var last30Days = DateTime.Now.AddDays(-30);
            var adherencePercentage = await CalculateAdherencePercentageAsync(patientMedicationId, last30Days, DateTime.Now);
            
            var missedDoses = await _context.Set<MedicationAdherence>()
                .Where(ma => ma.PatientMedicationId == patientMedicationId &&
                           ma.ScheduledDateTime >= last30Days &&
                           ma.Status == "Missed")
                .CountAsync();

            patientMedication.AdherencePercentage = adherencePercentage;
            patientMedication.MissedDoses = missedDoses;
            patientMedication.LastAdherenceUpdate = DateTime.UtcNow;

            _context.Set<PatientMedication>().Update(patientMedication);
        }
    }
}
