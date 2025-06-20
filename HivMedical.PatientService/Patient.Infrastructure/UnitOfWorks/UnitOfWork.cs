using Microsoft.EntityFrameworkCore.Storage;
using Patient.Infrastructure.Data;
using Patient.Infrastructure.Repositories;

namespace Patient.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PatientDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        public UnitOfWork(PatientDbContext context)
        {
            _context = context;
            Patients = new PatientRepository(_context);
            MedicalRecords = new MedicalRecordRepository(_context);
            Appointments = new AppointmentRepository(_context);
        }

        public IPatientRepository Patients { get; private set; }
        public IMedicalRecordRepository MedicalRecords { get; private set; }
        public IAppointmentRepository Appointments { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
