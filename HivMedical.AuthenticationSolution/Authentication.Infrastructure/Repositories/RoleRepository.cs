using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Response;
using System.Linq.Expressions;

namespace Authentication.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext _context;

        public RoleRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.OrderBy(r => r.Id).ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Role> GetRoleByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
        }

        // IGenericRepository implementation
        public async Task<Role> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetByAsync(Expression<Func<Role, bool>> predicate)
        {
            return await _context.Roles.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Role>> GetManyAsync(Expression<Func<Role, bool>> predicate)
        {
            return await _context.Roles.Where(predicate).ToListAsync();
        }

        public async Task<ApiResponse<Role>> CreateAsync(Role entity)
        {
            try
            {
                await _context.Roles.AddAsync(entity);
                return new ApiResponse<Role> { Success = true, Data = entity, Message = "Role created successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Role> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<Role>> UpdateAsync(Role entity)
        {
            try
            {
                _context.Roles.Update(entity);
                return new ApiResponse<Role> { Success = true, Data = entity, Message = "Role updated successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Role> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse<bool> { Success = false, Message = "Role not found" };

                _context.Roles.Remove(entity);
                return new ApiResponse<bool> { Success = true, Data = true, Message = "Role deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Role entity)
        {
            try
            {
                _context.Roles.Remove(entity);
                return new ApiResponse<bool> { Success = true, Data = true, Message = "Role deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }
    }
}
