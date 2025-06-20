using Authentication.Domain.Entities;
using SharedLibrary.Interfaces;

namespace Authentication.Application.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> GetRoleByNameAsync(string name);
    }
}
