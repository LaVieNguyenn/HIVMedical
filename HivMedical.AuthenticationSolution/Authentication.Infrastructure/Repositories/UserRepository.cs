using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Interfaces;
using SharedLibrary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<User>> CreateAsync(User entity)
        {
            try
            {
                await _context.Users.AddAsync(entity);
                return new ApiResponse<User> { Success = true, Data = entity, Message = "User created successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<User> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse<bool> { Success = false, Message = "Entity not found" };

                _context.Users.Remove(entity);
                return new ApiResponse<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.FirstOrDefaultAsync(predicate);
        }

        public async Task<User> GetUserWithRoleByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserWithRoleByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<ApiResponse<User>> UpdateAsync(User entity)
        {
            try
            {
                _context.Users.Update(entity);
                return new ApiResponse<User> { Success = true, Data = entity, Message = "User updated successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<User> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(User entity)
        {
            try
            {
                _context.Users.Remove(entity);
                return new ApiResponse<bool> { Success = true, Data = true, Message = "User deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<IEnumerable<User>> GetManyAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.Where(predicate).ToListAsync();
        }
    }
}
