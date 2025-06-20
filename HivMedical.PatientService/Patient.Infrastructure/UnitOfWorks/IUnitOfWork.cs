using Patient.Infrastructure.Repositories;

namespace Patient.Infrastructure.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IPatientRepository Patients { get; }
        IMedicalRecordRepository MedicalRecords { get; }
        IAppointmentRepository Appointments { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
