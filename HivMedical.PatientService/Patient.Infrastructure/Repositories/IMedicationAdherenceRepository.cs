using Patient.Domain.Entities;
using SharedLibrary.Interfaces;

namespace Patient.Infrastructure.Repositories
{
    public interface IMedicationAdherenceRepository : IGenericRepository<MedicationAdherence>
    {
        Task<IEnumerable<MedicationAdherence>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicationAdherence>> GetByPatientMedicationIdAsync(int patientMedicationId);
        Task<IEnumerable<MedicationAdherence>> GetByMedicationIdAsync(int medicationId);
        Task<IEnumerable<MedicationAdherence>> GetByDateRangeAsync(int patientId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<MedicationAdherence>> GetByStatusAsync(string status);
        Task<IEnumerable<MedicationAdherence>> GetMissedDosesAsync(int patientId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<MedicationAdherence>> GetFilteredAdherenceAsync(
            int? patientId = null,
            int? medicationId = null,
            int? patientMedicationId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null,
            int pageNumber = 1,
            int pageSize = 10);
        Task<int> GetTotalFilteredCountAsync(
            int? patientId = null,
            int? medicationId = null,
            int? patientMedicationId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null);
        Task<MedicationAdherence?> GetByScheduledDateTimeAsync(int patientMedicationId, DateTime scheduledDateTime);
        Task<int> GetTotalScheduledDosesAsync(int patientId, DateTime fromDate, DateTime toDate);
        Task<int> GetTakenDosesCountAsync(int patientId, DateTime fromDate, DateTime toDate);
        Task<int> GetMissedDosesCountAsync(int patientId, DateTime fromDate, DateTime toDate);
        Task<decimal> CalculateAdherencePercentageAsync(int patientId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<string>> GetCommonMissedReasonsAsync(int patientId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<string>> GetReportedSideEffectsAsync(int patientId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<MedicationAdherence>> GetRecentAdherenceAsync(int patientId, int days = 7);
    }
}
