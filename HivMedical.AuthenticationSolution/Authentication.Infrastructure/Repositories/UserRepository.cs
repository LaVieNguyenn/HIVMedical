using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Response;
using System.Linq.Expressions;

namespace Authentication.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context; 
        }

        public Task<ApiResponse<User>> CreateAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<User>> DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByAsync(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserWithRoleByEmailAsync(string email)
        {
            return _context.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Email == email);
        }

        public Task<ApiResponse<User>> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
