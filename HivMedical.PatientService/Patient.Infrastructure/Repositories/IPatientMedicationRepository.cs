using Patient.Domain.Entities;
using SharedLibrary.Interfaces;

namespace Patient.Infrastructure.Repositories
{
    public interface IPatientMedicationRepository : IGenericRepository<PatientMedication>
    {
        Task<IEnumerable<PatientMedication>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<PatientMedication>> GetByMedicationIdAsync(int medicationId);
        Task<IEnumerable<PatientMedication>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<PatientMedication>> GetActiveByPatientIdAsync(int patientId);
        Task<IEnumerable<PatientMedication>> GetCurrentMedicationsByPatientIdAsync(int patientId);
        Task<IEnumerable<PatientMedication>> GetByStatusAsync(string status);
        Task<IEnumerable<PatientMedication>> GetUpcomingRefillsAsync(int days = 7);
        Task<IEnumerable<PatientMedication>> GetFilteredPatientMedicationsAsync(
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
            int pageSize = 10);
        Task<int> GetTotalFilteredCountAsync(
            int? patientId = null,
            int? medicationId = null,
            int? prescribedByDoctorId = null,
            string? status = null,
            bool? isCurrentlyTaking = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            string? category = null,
            string? medicationType = null);
        Task<PatientMedication?> GetWithMedicationAsync(int id);
        Task<IEnumerable<PatientMedication>> GetWithMedicationByPatientIdAsync(int patientId);
        Task<bool> HasActiveConflictingMedicationAsync(int patientId, int medicationId, int? excludeId = null);
        Task<decimal> CalculateAdherencePercentageAsync(int patientMedicationId, DateTime fromDate, DateTime toDate);
        Task UpdateAdherenceStatsAsync(int patientMedicationId);
    }
}
