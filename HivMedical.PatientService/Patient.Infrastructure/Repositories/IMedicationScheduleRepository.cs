using Patient.Domain.Entities;
using SharedLibrary.Interfaces;

namespace Patient.Infrastructure.Repositories
{
    public interface IMedicationScheduleRepository : IGenericRepository<MedicationSchedule>
    {
        Task<IEnumerable<MedicationSchedule>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicationSchedule>> GetByPatientMedicationIdAsync(int patientMedicationId);
        Task<IEnumerable<MedicationSchedule>> GetByMedicationIdAsync(int medicationId);
        Task<IEnumerable<MedicationSchedule>> GetActiveByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicationSchedule>> GetTodayScheduleByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicationSchedule>> GetScheduleByDateAndPatientAsync(int patientId, DateTime date);
        Task<IEnumerable<MedicationSchedule>> GetUpcomingRemindersAsync(DateTime fromTime, DateTime toTime);
        Task<IEnumerable<MedicationSchedule>> GetDailyScheduleAsync(string dayOfWeek);
        Task<IEnumerable<MedicationSchedule>> GetWithMedicationByPatientIdAsync(int patientId);
        Task<bool> HasConflictingScheduleAsync(int patientId, TimeSpan scheduledTime, string dayOfWeek, int? excludeId = null);
    }
}
