using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Doctor.Domain.Entities;

namespace Doctor.Application.Service
{
    public class DoctorService 
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllAsync()
        {
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            return doctors.Select(d => new DoctorDto
            {
                DoctorId = d.DoctorId,
                UserId = d.UserId,
                FullName = d.User?.FullName,
                Phone = d.User?.Phone,
                Email = d.User?.Email,
                Qualifications = d.Qualifications?.Select(q => q.Qualification?.Name ?? "").ToList(),
                Specializations = d.Specializations?.Select(s => s.Specialization?.Name ?? "").ToList()
            });
        }

        public async Task<DoctorDto?> GetByIdAsync(int id)
        {
            var d = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (d == null) return null;

            return new DoctorDto
            {
                DoctorId = d.DoctorId,
                UserId = d.UserId,
                FullName = d.User?.FullName,
                Phone = d.User?.Phone,
                Email = d.User?.Email,
                Qualifications = d.Qualifications?.Select(q => q.QualificationId.ToString()).ToList(),
                Specializations = d.Specializations?.Select(s => s.SpecializationId.ToString()).ToList()
            };
        }

        public async Task<DoctorDto> CreateAsync(DoctorDto dto)
        {
            var entity = new Doctors
            {
                UserId = dto.UserId,
                Qualifications = dto.Qualifications?.Select(q => new DoctorQualification { QualificationId = int.Parse(q) }).ToList(),
                Specializations = dto.Specializations?.Select(s => new DoctorSpecialization { SpecializationId = int.Parse(s) }).ToList()
            };

            await _unitOfWork.Doctors.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            dto.DoctorId = entity.DoctorId;
            return dto;
        }

        public async Task UpdateAsync(DoctorDto dto)
        {
            var entity = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId);
            if (entity == null) return;

            entity.Qualifications = dto.Qualifications?.Select(q => new DoctorQualification { QualificationId = int.Parse(q) }).ToList();
            entity.Specializations = dto.Specializations?.Select(s => new DoctorSpecialization { SpecializationId = int.Parse(s) }).ToList();

            _unitOfWork.Doctors.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor != null)
            {
                _unitOfWork.Doctors.Remove(doctor);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}