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
                Qualifications = d.Qualifications?.Select(q =>
                    q.Qualification == null ? "" : $"{q.QualificationId}:{q.Qualification.Name}").ToList(),
                Specializations = d.Specializations?.Select(s =>
                    s.Specialization == null ? "" : $"{s.SpecializationId}:{s.Specialization.Name}").ToList()
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
                Qualifications = d.Qualifications?.Select(q =>
                    q.Qualification == null ? "" : $"{q.QualificationId}:{q.Qualification.Name}").ToList(),
                Specializations = d.Specializations?.Select(s =>
                    s.Specialization == null ? "" : $"{s.SpecializationId}:{s.Specialization.Name}").ToList()
            };
        }

        public async Task<DoctorDto> CreateAsync(DoctorCreateDto dto)
        {
            var entity = new Doctors
            {
                UserId = dto.UserId,
                Qualifications = dto.Qualifications?.Select(q =>
                {
                    var arr = q.Split(':');
                    if (arr.Length == 0 || !int.TryParse(arr[0], out int id)) return null;
                    return new DoctorQualification { QualificationId = id };
                }).Where(x => x != null).ToList(),
                Specializations = dto.Specializations?.Select(s =>
                {
                    var arr = s.Split(':');
                    if (arr.Length == 0 || !int.TryParse(arr[0], out int id)) return null;
                    return new DoctorSpecialization { SpecializationId = id };
                }).Where(x => x != null).ToList()
            };

            await _unitOfWork.Doctors.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Return DoctorDto mới tạo
            return new DoctorDto
            {
                DoctorId = entity.DoctorId,
                UserId = entity.UserId,
                Qualifications = entity.Qualifications?.Select(q => q.QualificationId.ToString()).ToList(),
                Specializations = entity.Specializations?.Select(s => s.SpecializationId.ToString()).ToList(),
            };
        }

        public async Task<bool> UpdateAsync(int id, DoctorUpdateDto dto)
        {
            var entity = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (entity == null) return false;

            entity.Qualifications = dto.Qualifications?.Select(q =>
            {
                var arr = q.Split(':');
                if (arr.Length == 0 || !int.TryParse(arr[0], out int qid)) return null;
                return new DoctorQualification { QualificationId = qid };
            }).Where(x => x != null).ToList();

            entity.Specializations = dto.Specializations?.Select(s =>
            {
                var arr = s.Split(':');
                if (arr.Length == 0 || !int.TryParse(arr[0], out int sid)) return null;
                return new DoctorSpecialization { SpecializationId = sid };
            }).Where(x => x != null).ToList();

            await _unitOfWork.Doctors.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor == null) return false;

            await _unitOfWork.Doctors.DeleteAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
