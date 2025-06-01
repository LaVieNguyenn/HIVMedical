using Doctor.Application.Interfaces;
using Doctor.Infrastructure.Data;

namespace Doctor.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DoctorDbContext _context;
        public UnitOfWork(DoctorDbContext context) => _context = context;

        public IDoctorRepository Doctors => new DoctorRepository(_context);

        public void Dispose() => _context.Dispose();

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}