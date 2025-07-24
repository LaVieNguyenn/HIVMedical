using Microsoft.EntityFrameworkCore;
using Patient.Application.DTOs;
using Patient.Domain.Entities;
using Patient.Infrastructure.Data;
using SharedLibrary.Response;

namespace Patient.Application.Services
{
    public class DoctorService
    {
        private readonly PatientDbContext _dbContext;

        public DoctorService(PatientDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<List<DoctorDto>>> GetAllDoctorsAsync()
        {
            try
            {
                var doctors = await _dbContext.Doctors
                    .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.FullName)
                    .Select(d => new DoctorDto
                    {
                        Id = d.Id,
                        FullName = d.FullName,
                        Specialization = d.Specialization,
                        LicenseNumber = d.LicenseNumber,
                        PhoneNumber = d.PhoneNumber,
                        Email = d.Email
                    })
                    .ToListAsync();

                return ApiResponse<List<DoctorDto>>.CreateSuccess(doctors);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DoctorDto>>.Failure($"Error retrieving doctors: {ex.Message}");
            }
        }

        public async Task<ApiResponse<DoctorDto>> GetDoctorByIdAsync(int id)
        {
            try
            {
                var doctor = await _dbContext.Doctors
                    .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

                if (doctor == null)
                    return ApiResponse<DoctorDto>.Failure("Doctor not found");

                var doctorDto = new DoctorDto
                {
                    Id = doctor.Id,
                    FullName = doctor.FullName,
                    Specialization = doctor.Specialization,
                    LicenseNumber = doctor.LicenseNumber,
                    PhoneNumber = doctor.PhoneNumber,
                    Email = doctor.Email
                };

                return ApiResponse<DoctorDto>.CreateSuccess(doctorDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<DoctorDto>.Failure($"Error retrieving doctor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<DoctorDto>> GetDoctorByAuthUserIdAsync(int authUserId)
        {
            try
            {
                var doctor = await _dbContext.Doctors
                    .FirstOrDefaultAsync(d => d.AuthUserId == authUserId && !d.IsDeleted);

                if (doctor == null)
                    return ApiResponse<DoctorDto>.Failure("Doctor not found");

                var doctorDto = new DoctorDto
                {
                    Id = doctor.Id,
                    FullName = doctor.FullName,
                    Specialization = doctor.Specialization,
                    LicenseNumber = doctor.LicenseNumber,
                    PhoneNumber = doctor.PhoneNumber,
                    Email = doctor.Email
                };

                return ApiResponse<DoctorDto>.CreateSuccess(doctorDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<DoctorDto>.Failure($"Error retrieving doctor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<DoctorDto>> CreateDoctorAsync(CreateDoctorRequest request)
        {
            try
            {
                // Check if doctor with AuthUserId already exists
                var existingDoctor = await _dbContext.Doctors
                    .FirstOrDefaultAsync(d => d.AuthUserId == request.AuthUserId && !d.IsDeleted);

                if (existingDoctor != null)
                    return ApiResponse<DoctorDto>.Failure("Doctor with this Auth User ID already exists");

                var doctor = new Doctor
                {
                    AuthUserId = request.AuthUserId,
                    FullName = request.FullName,
                    Specialization = request.Specialization,
                    LicenseNumber = request.LicenseNumber,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Doctors.Add(doctor);
                await _dbContext.SaveChangesAsync();

                var doctorDto = new DoctorDto
                {
                    Id = doctor.Id,
                    FullName = doctor.FullName,
                    Specialization = doctor.Specialization,
                    LicenseNumber = doctor.LicenseNumber,
                    PhoneNumber = doctor.PhoneNumber,
                    Email = doctor.Email
                };

                return ApiResponse<DoctorDto>.CreateSuccess(doctorDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<DoctorDto>.Failure($"Error creating doctor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<DoctorDto>> UpdateDoctorAsync(int id, UpdateDoctorRequest request)
        {
            try
            {
                var doctor = await _dbContext.Doctors.FindAsync(id);
                if (doctor == null || doctor.IsDeleted)
                    return ApiResponse<DoctorDto>.Failure("Doctor not found");

                doctor.FullName = request.FullName;
                doctor.Specialization = request.Specialization;
                doctor.LicenseNumber = request.LicenseNumber;
                doctor.PhoneNumber = request.PhoneNumber;
                doctor.Email = request.Email;
                doctor.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                var doctorDto = new DoctorDto
                {
                    Id = doctor.Id,
                    FullName = doctor.FullName,
                    Specialization = doctor.Specialization,
                    LicenseNumber = doctor.LicenseNumber,
                    PhoneNumber = doctor.PhoneNumber,
                    Email = doctor.Email
                };

                return ApiResponse<DoctorDto>.CreateSuccess(doctorDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<DoctorDto>.Failure($"Error updating doctor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteDoctorAsync(int id)
        {
            try
            {
                var doctor = await _dbContext.Doctors.FindAsync(id);
                if (doctor == null || doctor.IsDeleted)
                    return ApiResponse<bool>.Failure("Doctor not found");

                // Check if there are appointments linked to this doctor
                var hasAppointments = await _dbContext.Appointments
                    .AnyAsync(a => a.DoctorId == id && !a.IsDeleted);

                if (hasAppointments)
                    return ApiResponse<bool>.Failure("Cannot delete doctor with existing appointments");

                doctor.IsDeleted = true;
                doctor.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return ApiResponse<bool>.CreateSuccess(true, "Doctor successfully deleted");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Error deleting doctor: {ex.Message}");
            }
        }
    }
}


