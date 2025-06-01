using Doctor.Domain.Entities;
using SharedLibrary.Interfaces;
using System.Threading.Tasks;

namespace Doctor.Application.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctors>
    {
        Task<IEnumerable<Doctors>> GetAllAsync();
        Task<Doctors?> GetByIdAsync(int id);

        Task AddAsync(Doctors doctor);
        void Update(Doctors doctor);
        void Remove(Doctors doctor);
    }
}
