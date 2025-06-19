using Authentication.Domain.Entities;
using SharedLibrary.Interfaces;
using SharedLibrary.Response;

namespace Authentication.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserWithRoleByEmailAsync(string email);
        Task<User> GetUserWithRoleByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task<ApiResponse<User>> CreateAsync(User entity);
        Task<ApiResponse<User>> UpdateAsync(User entity);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
