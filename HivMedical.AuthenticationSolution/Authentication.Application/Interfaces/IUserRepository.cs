using Authentication.Domain.Entities;
using SharedLibrary.Interfaces;

namespace Authentication.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserWithRoleByEmailAsync(string email);
    }
}
