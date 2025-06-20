using Authentication.Application.DTOs;
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

        // Admin user management methods
        public async Task<PagedUserListResponse> GetUsersWithPaginationAsync(UserFilterRequest filter)
        {
            var query = _context.Users.Include(u => u.Role).AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(u => u.FullName.Contains(filter.SearchTerm) ||
                                        u.Email.Contains(filter.SearchTerm) ||
                                        u.UserName.Contains(filter.SearchTerm));
            }

            if (filter.RoleId.HasValue)
            {
                query = query.Where(u => u.RoleId == filter.RoleId.Value);
            }

            if (filter.IsDeleted.HasValue)
            {
                query = query.Where(u => u.IsDeleted == filter.IsDeleted.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            var users = await query
                .OrderBy(u => u.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsAnonymous = u.IsAnonymous,
                    Role = u.Role.Name,
                    RoleId = u.RoleId,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    IsDeleted = u.IsDeleted
                })
                .ToListAsync();

            return new PagedUserListResponse
            {
                Users = users,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages,
                HasNextPage = filter.PageNumber < totalPages,
                HasPreviousPage = filter.PageNumber > 1
            };
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId == roleId && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<ApiResponse<bool>> SoftDeleteAsync(int id)
        {
            try
            {
                var user = await GetByIdAsync(id);
                if (user == null)
                    return new ApiResponse<bool> { Success = false, Message = "User not found" };

                user.IsDeleted = true;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                return new ApiResponse<bool> { Success = true, Data = true, Message = "User deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> RestoreUserAsync(int id)
        {
            try
            {
                var user = await GetByIdAsync(id);
                if (user == null)
                    return new ApiResponse<bool> { Success = false, Message = "User not found" };

                user.IsDeleted = false;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                return new ApiResponse<bool> { Success = true, Data = true, Message = "User restored successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<bool> EmailExistsForOtherUserAsync(string email, int userId)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && u.Id != userId);
        }

        public async Task<IEnumerable<User>> GetAllUsersWithRolesAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }
    }
}
