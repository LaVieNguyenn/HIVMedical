using Microsoft.EntityFrameworkCore;
using SharedLibrary.Interfaces;
using SharedLibrary.Response;
using System.Linq.Expressions;

namespace SharedLibrary.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext _context;

        public GenericRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<ApiResponse<T>> CreateAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                return new ApiResponse<T> { Success = true, Data = entity, Message = "Entity created successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<T> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<T>> UpdateAsync(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                return new ApiResponse<T> { Success = true, Data = entity, Message = "Entity updated successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<T> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                return new ApiResponse<bool> { Success = true, Data = true, Message = "Entity deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<T?> GetByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }
    }
}
