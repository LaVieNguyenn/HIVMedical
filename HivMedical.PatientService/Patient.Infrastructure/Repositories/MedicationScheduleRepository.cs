using Microsoft.EntityFrameworkCore;
using Patient.Domain.Entities;
using Patient.Infrastructure.Data;
using SharedLibrary.Repository;

namespace Patient.Infrastructure.Repositories
{
    public class MedicationScheduleRepository : GenericRepository<MedicationSchedule>, IMedicationScheduleRepository
    {
        public MedicationScheduleRepository(PatientDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MedicationSchedule>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Set<MedicationSchedule>()
                .Include(ms => ms.Medication)
                .Include(ms => ms.PatientMedication)
                .Where(ms => ms.PatientId == patientId)
                .OrderBy(ms => ms.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationSchedule>> GetByPatientMedicationIdAsync(int patientMedicationId)
        {
            return await _context.Set<MedicationSchedule>()
                .Include(ms => ms.Medication)
                .Include(ms => ms.PatientMedication)
                .Where(ms => ms.PatientMedicationId == patientMedicationId)
                .OrderBy(ms => ms.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationSchedule>> GetByMedicationIdAsync(int medicationId)
        {
            return await _context.Set<MedicationSchedule>()
                .Include(ms => ms.Medication)
                .Include(ms => ms.PatientMedication)
                .Where(ms => ms.MedicationId == medicationId)
                .OrderBy(ms => ms.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationSchedule>> GetActiveByPatientIdAsync(int patientId)
        {
            return await _context.Set<MedicationSchedule>()
                .Include(ms => ms.Medication)
                .Include(ms => ms.PatientMedication)
                .Where(ms => ms.PatientId == patientId && ms.IsActive)
                .OrderBy(ms => ms.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationSchedule>> GetTodayScheduleByPatientIdAsync(int patientId)
        {
            var today = DateTime.Now.DayOfWeek.ToString();
            return await _context.Set<MedicationSchedule>()
                .Include(ms => ms.Medication)
                .Include(ms => ms.PatientMedication)
                .Where(ms => ms.PatientId == patientId && 
                           ms.IsActive && 
                           (ms.DayOfWeek == "Daily" || ms.DayOfWeek == today))
                .OrderBy(ms => ms.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationSchedule>> GetScheduleByDateAndPatientAsync(int patientId, DateTime date)
        {
            var dayOfWeek = date.DayOfWeek.ToString();
            return await _context.Set<MedicationSchedule>()
                .Include(ms => ms.Medication)
                .Include(ms => ms.PatientMedication)
                .Where(ms => ms.PatientId == patientId && 
                           ms.IsActive && 
                           (ms.DayOfWeek == "Daily" || ms.DayOfWeek == dayOfWeek))
                .OrderBy(ms => ms.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationSchedule>> GetUpcomingRemindersAsync(DateTime fromTime, DateTime toTime)
        {
            var currentTime = fromTime.TimeOfDay;
            var endTime = toTime.TimeOfDay;
            var today = fromTime.DayOfWeek.ToString();

            return await _context.Set<MedicationSchedule>()
                .Include(ms => ms.Medication)
                .Include(ms => ms.PatientMedication)
                .Include(ms => ms.Patient)
                .Where(ms => ms.IsActive && 
                           ms.ReminderEnabled &&
                           (ms.DayOfWeek == "Daily" || ms.DayOfWeek == today) &&
                           ms.ScheduledTime >= currentTime && 
                           ms.ScheduledTime <= endTime)
                .OrderBy(ms => ms.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationSchedule>> GetDailyScheduleAsync(string dayOfWeek)
        {
            return await _context.Set<MedicationSchedule>()
                .Include(ms => ms.Medication)
                .Include(ms => ms.PatientMedication)
                .Include(ms => ms.Patient)
                .Where(ms => ms.IsActive && 
                           (ms.DayOfWeek == "Daily" || ms.DayOfWeek == dayOfWeek))
                .OrderBy(ms => ms.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationSchedule>> GetWithMedicationByPatientIdAsync(int patientId)
        {
            return await _context.Set<MedicationSchedule>()
                .Include(ms => ms.Medication)
                .Include(ms => ms.PatientMedication)
                .ThenInclude(pm => pm.Medication)
                .Where(ms => ms.PatientId == patientId)
                .OrderBy(ms => ms.ScheduledTime)
                .ToListAsync();
        }

        public async Task<bool> HasConflictingScheduleAsync(int patientId, TimeSpan scheduledTime, string dayOfWeek, int? excludeId = null)
        {
            var query = _context.Set<MedicationSchedule>()
                .Where(ms => ms.PatientId == patientId &&
                           ms.ScheduledTime == scheduledTime &&
                           ms.DayOfWeek == dayOfWeek &&
                           ms.IsActive);

            if (excludeId.HasValue)
                query = query.Where(ms => ms.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
