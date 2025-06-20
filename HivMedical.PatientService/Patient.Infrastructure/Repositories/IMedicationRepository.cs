using Patient.Domain.Entities;
using SharedLibrary.Interfaces;

namespace Patient.Infrastructure.Repositories
{
    public interface IMedicationRepository : IGenericRepository<Medication>
    {
        Task<IEnumerable<Medication>> GetByNameAsync(string name);
        Task<IEnumerable<Medication>> GetByCategoryAsync(string category);
        Task<IEnumerable<Medication>> GetByMedicationTypeAsync(string medicationType);
        Task<IEnumerable<Medication>> GetActiveAsync();
        Task<IEnumerable<Medication>> GetFilteredMedicationsAsync(
            string? name = null,
            string? category = null,
            string? medicationType = null,
            string? form = null,
            bool? isActive = null,
            int pageNumber = 1,
            int pageSize = 10);
        Task<int> GetTotalFilteredCountAsync(
            string? name = null,
            string? category = null,
            string? medicationType = null,
            string? form = null,
            bool? isActive = null);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Medication>> SearchByNameOrGenericAsync(string searchTerm);
        Task<IEnumerable<Medication>> GetHIVMedicationsAsync();
        Task<IEnumerable<Medication>> GetARVMedicationsAsync();
    }
}
