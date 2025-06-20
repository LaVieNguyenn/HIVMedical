using SharedLibrary.Interfaces;

namespace Patient.Infrastructure.Repositories
{
    public interface IPatientRepository : IGenericRepository<Domain.Entities.Patient>
    {
        Task<Domain.Entities.Patient?> GetByPatientCodeAsync(string patientCode);
        Task<Domain.Entities.Patient?> GetByAuthUserIdAsync(int authUserId);
        Task<Domain.Entities.Patient?> GetPatientWithRecordsAsync(int patientId);
        Task<Domain.Entities.Patient?> GetPatientWithAppointmentsAsync(int patientId);
        Task<IEnumerable<Domain.Entities.Patient>> GetPatientsByStatusAsync(string status);
        Task<bool> PatientCodeExistsAsync(string patientCode);
        Task<bool> AuthUserIdExistsAsync(int authUserId);
    }
}
