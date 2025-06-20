using Doctor.Domain.Entities;
using SharedLibrary.Interfaces;
using System.Threading.Tasks;

namespace Doctor.Application.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctors>
    {
        Task<List<Doctors>> GetAllAsync();
        Task<Doctors?> GetByIdAsync(int id);
        Task<Doctors> AddAsync(Doctors doctor);
        Task<bool> UpdateAsync(Doctors doctor);
        Task<bool> DeleteAsync(int id);
    }
}
