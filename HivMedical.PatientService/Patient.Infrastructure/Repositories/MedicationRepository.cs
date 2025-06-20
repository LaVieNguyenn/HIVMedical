using Microsoft.EntityFrameworkCore;
using Patient.Domain.Entities;
using Patient.Infrastructure.Data;
using SharedLibrary.Repository;

namespace Patient.Infrastructure.Repositories
{
    public class MedicationRepository : GenericRepository<Medication>, IMedicationRepository
    {
        public MedicationRepository(PatientDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Medication>> GetByNameAsync(string name)
        {
            return await _context.Set<Medication>()
                .Where(m => m.Name.Contains(name) || m.GenericName.Contains(name) || m.BrandName.Contains(name))
                .Where(m => m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetByCategoryAsync(string category)
        {
            return await _context.Set<Medication>()
                .Where(m => m.Category == category && m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetByMedicationTypeAsync(string medicationType)
        {
            return await _context.Set<Medication>()
                .Where(m => m.MedicationType == medicationType && m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetActiveAsync()
        {
            return await _context.Set<Medication>()
                .Where(m => m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetFilteredMedicationsAsync(
            string? name = null,
            string? category = null,
            string? medicationType = null,
            string? form = null,
            bool? isActive = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Set<Medication>().AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(m => m.Name.Contains(name) || m.GenericName.Contains(name) || m.BrandName.Contains(name));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(m => m.Category == category);

            if (!string.IsNullOrEmpty(medicationType))
                query = query.Where(m => m.MedicationType == medicationType);

            if (!string.IsNullOrEmpty(form))
                query = query.Where(m => m.Form == form);

            if (isActive.HasValue)
                query = query.Where(m => m.IsActive == isActive.Value);

            return await query
                .OrderBy(m => m.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalFilteredCountAsync(
            string? name = null,
            string? category = null,
            string? medicationType = null,
            string? form = null,
            bool? isActive = null)
        {
            var query = _context.Set<Medication>().AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(m => m.Name.Contains(name) || m.GenericName.Contains(name) || m.BrandName.Contains(name));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(m => m.Category == category);

            if (!string.IsNullOrEmpty(medicationType))
                query = query.Where(m => m.MedicationType == medicationType);

            if (!string.IsNullOrEmpty(form))
                query = query.Where(m => m.Form == form);

            if (isActive.HasValue)
                query = query.Where(m => m.IsActive == isActive.Value);

            return await query.CountAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Set<Medication>()
                .AnyAsync(m => m.Name == name || m.GenericName == name || m.BrandName == name);
        }

        public async Task<IEnumerable<Medication>> SearchByNameOrGenericAsync(string searchTerm)
        {
            return await _context.Set<Medication>()
                .Where(m => (m.Name.Contains(searchTerm) || 
                           m.GenericName.Contains(searchTerm) || 
                           m.BrandName.Contains(searchTerm)) && 
                           m.IsActive)
                .OrderBy(m => m.Name)
                .Take(20) // Limit search results
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetHIVMedicationsAsync()
        {
            return await _context.Set<Medication>()
                .Where(m => m.Category == "ARV" || m.Category == "HIV" && m.IsActive)
                .OrderBy(m => m.MedicationType)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetARVMedicationsAsync()
        {
            return await _context.Set<Medication>()
                .Where(m => m.Category == "ARV" && m.IsActive)
                .OrderBy(m => m.MedicationType)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }
    }
}
