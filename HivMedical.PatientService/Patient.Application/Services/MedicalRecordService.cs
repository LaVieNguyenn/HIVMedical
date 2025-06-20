using Patient.Application.DTOs;
using Patient.Domain.Entities;
using Patient.Infrastructure.UnitOfWorks;
using SharedLibrary.Response;

namespace Patient.Application.Services
{
    public class MedicalRecordService
    {
        private readonly IUnitOfWork _uow;

        public MedicalRecordService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<ApiResponse<MedicalRecordDto>> GetMedicalRecordByIdAsync(int id)
        {
            var record = await _uow.MedicalRecords.GetByIdAsync(id);
            if (record == null)
            {
                return new ApiResponse<MedicalRecordDto>
                {
                    Success = false,
                    Message = "Medical record not found"
                };
            }

            var recordDto = MapToDto(record);
            return new ApiResponse<MedicalRecordDto>
            {
                Success = true,
                Data = recordDto
            };
        }

        public async Task<ApiResponse<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsByPatientIdAsync(int patientId)
        {
            var records = await _uow.MedicalRecords.GetByPatientIdAsync(patientId);
            var recordDtos = records.Select(MapToDto);

            return new ApiResponse<IEnumerable<MedicalRecordDto>>
            {
                Success = true,
                Data = recordDtos
            };
        }

        public async Task<ApiResponse<MedicalRecordDto>> CreateMedicalRecordAsync(CreateMedicalRecordRequest request)
        {
            // Verify patient exists
            var patient = await _uow.Patients.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                return new ApiResponse<MedicalRecordDto>
                {
                    Success = false,
                    Message = "Patient not found"
                };
            }

            var record = new MedicalRecord
            {
                PatientId = request.PatientId,
                RecordDate = request.RecordDate,
                RecordType = request.RecordType,
                Title = request.Title,
                Description = request.Description,
                LabResults = request.LabResults,
                Medications = request.Medications,
                Notes = request.Notes,
                DoctorId = request.DoctorId,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _uow.MedicalRecords.CreateAsync(record);
            if (!result.Success)
            {
                return new ApiResponse<MedicalRecordDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var recordDto = MapToDto(record);
            return new ApiResponse<MedicalRecordDto>
            {
                Success = true,
                Message = "Medical record created successfully",
                Data = recordDto
            };
        }

        public async Task<ApiResponse<MedicalRecordDto>> UpdateMedicalRecordAsync(int id, UpdateMedicalRecordRequest request)
        {
            var record = await _uow.MedicalRecords.GetByIdAsync(id);
            if (record == null)
            {
                return new ApiResponse<MedicalRecordDto>
                {
                    Success = false,
                    Message = "Medical record not found"
                };
            }

            // Update record properties
            record.RecordDate = request.RecordDate;
            record.RecordType = request.RecordType;
            record.Title = request.Title;
            record.Description = request.Description;
            record.LabResults = request.LabResults;
            record.Medications = request.Medications;
            record.Notes = request.Notes;
            record.UpdatedAt = DateTime.UtcNow;

            var result = await _uow.MedicalRecords.UpdateAsync(record);
            if (!result.Success)
            {
                return new ApiResponse<MedicalRecordDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var recordDto = MapToDto(record);
            return new ApiResponse<MedicalRecordDto>
            {
                Success = true,
                Message = "Medical record updated successfully",
                Data = recordDto
            };
        }

        public async Task<ApiResponse<bool>> DeleteMedicalRecordAsync(int id)
        {
            var record = await _uow.MedicalRecords.GetByIdAsync(id);
            if (record == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Medical record not found"
                };
            }

            var result = await _uow.MedicalRecords.DeleteAsync(record);
            if (!result.Success)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Medical record deleted successfully",
                Data = true
            };
        }

        private static MedicalRecordDto MapToDto(MedicalRecord record)
        {
            return new MedicalRecordDto
            {
                Id = record.Id,
                PatientId = record.PatientId,
                RecordDate = record.RecordDate,
                RecordType = record.RecordType,
                Title = record.Title,
                Description = record.Description,
                LabResults = record.LabResults,
                Medications = record.Medications,
                Notes = record.Notes,
                DoctorId = record.DoctorId,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            };
        }
    }
}
