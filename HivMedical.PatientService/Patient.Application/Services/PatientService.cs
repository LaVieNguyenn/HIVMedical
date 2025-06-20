using Patient.Application.DTOs;
using Patient.Infrastructure.UnitOfWorks;
using SharedLibrary.Response;

namespace Patient.Application.Services
{
    public class PatientService
    {
        private readonly IUnitOfWork _uow;

        public PatientService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<ApiResponse<PatientDto>> GetPatientByIdAsync(int id)
        {
            var patient = await _uow.Patients.GetByIdAsync(id);
            if (patient == null)
            {
                return new ApiResponse<PatientDto>
                {
                    Success = false,
                    Message = "Patient not found"
                };
            }

            var patientDto = MapToDto(patient);
            return new ApiResponse<PatientDto>
            {
                Success = true,
                Data = patientDto
            };
        }

        public async Task<ApiResponse<PatientDto>> GetPatientByAuthUserIdAsync(int authUserId)
        {
            var patient = await _uow.Patients.GetByAuthUserIdAsync(authUserId);
            if (patient == null)
            {
                return new ApiResponse<PatientDto>
                {
                    Success = false,
                    Message = "Patient not found for this user"
                };
            }

            var patientDto = MapToDto(patient);
            return new ApiResponse<PatientDto>
            {
                Success = true,
                Data = patientDto
            };
        }

        public async Task<ApiResponse<PatientDto>> CreatePatientAsync(CreatePatientRequest request)
        {
            // Check if AuthUserId already exists
            if (await _uow.Patients.AuthUserIdExistsAsync(request.AuthUserId))
            {
                return new ApiResponse<PatientDto>
                {
                    Success = false,
                    Message = "Patient already exists for this user"
                };
            }

            // Generate unique patient code
            var patientCode = await GeneratePatientCodeAsync();

            var patient = new Domain.Entities.Patient
            {
                PatientCode = patientCode,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Address = request.Address,
                EmergencyContact = request.EmergencyContact,
                EmergencyPhone = request.EmergencyPhone,
                AuthUserId = request.AuthUserId,
                DiagnosisDate = request.DiagnosisDate,
                HIVStatus = request.HIVStatus ?? "Unknown",
                TreatmentStatus = request.TreatmentStatus ?? "Not on Treatment",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _uow.Patients.CreateAsync(patient);
            if (!result.Success)
            {
                return new ApiResponse<PatientDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var patientDto = MapToDto(patient);
            return new ApiResponse<PatientDto>
            {
                Success = true,
                Message = "Patient created successfully",
                Data = patientDto
            };
        }

        public async Task<ApiResponse<PatientDto>> UpdatePatientAsync(int id, UpdatePatientRequest request)
        {
            var patient = await _uow.Patients.GetByIdAsync(id);
            if (patient == null)
            {
                return new ApiResponse<PatientDto>
                {
                    Success = false,
                    Message = "Patient not found"
                };
            }

            // Update patient properties
            patient.FullName = request.FullName;
            patient.Email = request.Email;
            patient.Phone = request.Phone;
            patient.DateOfBirth = request.DateOfBirth;
            patient.Gender = request.Gender;
            patient.Address = request.Address;
            patient.EmergencyContact = request.EmergencyContact;
            patient.EmergencyPhone = request.EmergencyPhone;
            patient.DiagnosisDate = request.DiagnosisDate;
            patient.HIVStatus = request.HIVStatus;
            patient.TreatmentStatus = request.TreatmentStatus;
            patient.UpdatedAt = DateTime.UtcNow;

            var result = await _uow.Patients.UpdateAsync(patient);
            if (!result.Success)
            {
                return new ApiResponse<PatientDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var patientDto = MapToDto(patient);
            return new ApiResponse<PatientDto>
            {
                Success = true,
                Message = "Patient updated successfully",
                Data = patientDto
            };
        }

        public async Task<ApiResponse<IEnumerable<PatientDto>>> GetAllPatientsAsync()
        {
            var patients = await _uow.Patients.GetAllAsync();
            var patientDtos = patients.Select(MapToDto);

            return new ApiResponse<IEnumerable<PatientDto>>
            {
                Success = true,
                Data = patientDtos
            };
        }

        private async Task<string> GeneratePatientCodeAsync()
        {
            string patientCode;
            do
            {
                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
                var random = new Random().Next(1000, 9999);
                patientCode = $"PT{timestamp}{random}";
            }
            while (await _uow.Patients.PatientCodeExistsAsync(patientCode));

            return patientCode;
        }

        private static PatientDto MapToDto(Domain.Entities.Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                PatientCode = patient.PatientCode,
                FullName = patient.FullName,
                Email = patient.Email,
                Phone = patient.Phone,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                EmergencyContact = patient.EmergencyContact,
                EmergencyPhone = patient.EmergencyPhone,
                IsActive = patient.IsActive,
                AuthUserId = patient.AuthUserId,
                DiagnosisDate = patient.DiagnosisDate,
                HIVStatus = patient.HIVStatus,
                TreatmentStatus = patient.TreatmentStatus,
                CreatedAt = patient.CreatedAt,
                UpdatedAt = patient.UpdatedAt
            };
        }
    }
}
