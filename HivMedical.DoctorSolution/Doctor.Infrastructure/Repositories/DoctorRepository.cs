using Doctor.Application.Interfaces;
using Doctor.Domain.Entities;
using Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Response;
using System.Linq.Expressions;

public class DoctorRepository : IDoctorRepository
{
    private readonly DoctorDbContext _context;
    public DoctorRepository(DoctorDbContext context) => _context = context;

    public async Task<IEnumerable<Doctors>> GetAllAsync() => await _context.Doctors
        .Include(d => d.User)
        .Include(d => d.Qualifications)
        .ThenInclude(q => q.Qualification)
        .Include(d => d.Specializations)
        .ThenInclude(s => s.Specialization)
        .ToListAsync();

    public async Task<Doctors?> GetByIdAsync(int id)
    {
        return await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Qualifications)
            .Include(d => d.Specializations)
            .FirstOrDefaultAsync(d => d.DoctorId == id);
    }

    public async Task AddAsync(Doctors doctor) => await _context.Doctors.AddAsync(doctor);

    public void Update(Doctors doctor) => _context.Doctors.Update(doctor);

    public void Remove(Doctors doctor) => _context.Doctors.Remove(doctor);

    public Task<ApiResponse<Doctors>> CreateAsync(Doctors entity)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<Doctors>> UpdateAsync(Doctors entity)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<Doctors>> DeleteAsync(Doctors entity)
    {
        throw new NotImplementedException();
    }

    public Task<Doctors> FindByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Doctors> GetByAsync(Expression<Func<Doctors, bool>> predicate)
    {
        throw new NotImplementedException();
    }
}