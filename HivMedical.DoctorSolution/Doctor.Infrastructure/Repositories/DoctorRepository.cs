using Doctor.Application.Interfaces;
using Doctor.Domain.Entities;
using Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Interfaces;
using SharedLibrary.Response;
using System.Linq.Expressions;

public class DoctorsRepository : IDoctorRepository
{
    private readonly DoctorDbContext _context;

    public DoctorsRepository(DoctorDbContext context)
    {
        _context = context;
    }

    public async Task<List<Doctors>> GetAllAsync()
    {
        return await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Qualifications).ThenInclude(q => q.Qualification)
            .Include(d => d.Specializations).ThenInclude(s => s.Specialization)
            .ToListAsync();
    }

    public async Task<Doctors?> GetByIdAsync(int id)
    {
        return await _context.Doctors
            .Include(d => d.User)
            .Include(d => d.Qualifications).ThenInclude(q => q.Qualification)
            .Include(d => d.Specializations).ThenInclude(s => s.Specialization)
            .FirstOrDefaultAsync(d => d.DoctorId == id);
    }

    public async Task<Doctors> AddAsync(Doctors doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }

    public async Task<bool> UpdateAsync(Doctors doctor)
    {
        var existing = await _context.Doctors.FindAsync(doctor.DoctorId);
        if (existing == null) return false;
        _context.Entry(existing).CurrentValues.SetValues(doctor);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return false;
        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<ApiResponse<Doctors>> CreateAsync(Doctors entity)
    {
        throw new NotImplementedException();
    }

    Task<ApiResponse<Doctors>> IGenericRepository<Doctors>.UpdateAsync(Doctors entity)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<Doctors>> DeleteAsync(Doctors entity)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<Doctors>> IGenericRepository<Doctors>.GetAllAsync()
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
