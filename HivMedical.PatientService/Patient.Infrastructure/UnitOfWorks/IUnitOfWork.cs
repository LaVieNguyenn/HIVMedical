using Patient.Infrastructure.Repositories;

namespace Patient.Infrastructure.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IPatientRepository Patients { get; }
        IMedicalRecordRepository MedicalRecords { get; }
        IAppointmentRepository Appointments { get; }
        IMedicationRepository Medications { get; }
        IPatientMedicationRepository PatientMedications { get; }
        IMedicationScheduleRepository MedicationSchedules { get; }
        IMedicationAdherenceRepository MedicationAdherences { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
