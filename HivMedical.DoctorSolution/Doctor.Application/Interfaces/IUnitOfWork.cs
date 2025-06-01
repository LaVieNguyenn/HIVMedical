namespace Doctor.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDoctorRepository Doctors { get; }
        Task<int> SaveChangesAsync();
    }
}
