using Patient.Application.DTOs;
using Patient.Domain.Entities;
using Patient.Infrastructure.UnitOfWorks;
using SharedLibrary.Response;

namespace Patient.Application.Services
{
    public class PatientMedicationService
    {
        private readonly IUnitOfWork _uow;

        public PatientMedicationService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<ApiResponse<PatientMedicationDto>> GetPatientMedicationByIdAsync(int id)
        {
            var patientMedication = await _uow.PatientMedications.GetWithMedicationAsync(id);
            if (patientMedication == null)
            {
                return new ApiResponse<PatientMedicationDto>
                {
                    Success = false,
                    Message = "Patient medication not found"
                };
            }

            var dto = MapToDto(patientMedication);
            return new ApiResponse<PatientMedicationDto>
            {
                Success = true,
                Data = dto
            };
        }

        public async Task<ApiResponse<IEnumerable<PatientMedicationDto>>> GetPatientMedicationsByPatientIdAsync(int patientId)
        {
            var patientMedications = await _uow.PatientMedications.GetWithMedicationByPatientIdAsync(patientId);
            var dtos = patientMedications.Select(MapToDto);

            return new ApiResponse<IEnumerable<PatientMedicationDto>>
            {
                Success = true,
                Data = dtos
            };
        }

        public async Task<ApiResponse<IEnumerable<PatientMedicationDto>>> GetCurrentMedicationsByPatientIdAsync(int patientId)
        {
            var patientMedications = await _uow.PatientMedications.GetCurrentMedicationsByPatientIdAsync(patientId);
            var dtos = patientMedications.Select(MapToDto);

            return new ApiResponse<IEnumerable<PatientMedicationDto>>
            {
                Success = true,
                Data = dtos
            };
        }

        public async Task<ApiResponse<PatientMedicationSummaryDto>> GetPatientMedicationSummaryAsync(int patientId)
        {
            var allMedications = await _uow.PatientMedications.GetWithMedicationByPatientIdAsync(patientId);
            var currentMedications = await _uow.PatientMedications.GetCurrentMedicationsByPatientIdAsync(patientId);
            var upcomingRefills = await _uow.PatientMedications.GetUpcomingRefillsAsync(30);

            var summary = new PatientMedicationSummaryDto
            {
                TotalMedications = allMedications.Count(),
                ActiveMedications = allMedications.Count(pm => pm.Status == "Active"),
                CompletedMedications = allMedications.Count(pm => pm.Status == "Completed"),
                DiscontinuedMedications = allMedications.Count(pm => pm.Status == "Discontinued"),
                OverallAdherencePercentage = allMedications.Where(pm => pm.AdherencePercentage.HasValue)
                    .Average(pm => pm.AdherencePercentage),
                TotalMissedDoses = allMedications.Sum(pm => pm.MissedDoses ?? 0),
                LastAdherenceUpdate = allMedications.Max(pm => pm.LastAdherenceUpdate),
                CurrentMedications = currentMedications.Select(MapToDto).ToList(),
                UpcomingRefills = upcomingRefills.Where(pm => pm.PatientId == patientId).Select(MapToDto).ToList()
            };

            return new ApiResponse<PatientMedicationSummaryDto>
            {
                Success = true,
                Data = summary
            };
        }

        public async Task<ApiResponse<PatientMedicationDto>> PrescribeMedicationAsync(CreatePatientMedicationRequest request)
        {
            // Verify patient exists
            var patient = await _uow.Patients.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                return new ApiResponse<PatientMedicationDto>
                {
                    Success = false,
                    Message = "Patient not found"
                };
            }

            // Verify medication exists
            var medication = await _uow.Medications.GetByIdAsync(request.MedicationId);
            if (medication == null)
            {
                return new ApiResponse<PatientMedicationDto>
                {
                    Success = false,
                    Message = "Medication not found"
                };
            }

            // Check for conflicting active medications
            var hasConflict = await _uow.PatientMedications.HasActiveConflictingMedicationAsync(
                request.PatientId, request.MedicationId);
            
            if (hasConflict)
            {
                return new ApiResponse<PatientMedicationDto>
                {
                    Success = false,
                    Message = "Patient is already taking this medication"
                };
            }

            var patientMedication = new PatientMedication
            {
                PatientId = request.PatientId,
                MedicationId = request.MedicationId,
                PrescribedByDoctorId = request.PrescribedByDoctorId,
                PrescribedDate = request.PrescribedDate,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Dosage = request.Dosage,
                Frequency = request.Frequency,
                Instructions = request.Instructions,
                Status = "Active",
                Notes = request.Notes,
                IsCurrentlyTaking = true,
                RefillsRemaining = request.RefillsRemaining,
                NextRefillDue = request.NextRefillDue,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _uow.PatientMedications.CreateAsync(patientMedication);
            if (!result.Success)
            {
                return new ApiResponse<PatientMedicationDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            // Get the created medication with related data
            var createdMedication = await _uow.PatientMedications.GetWithMedicationAsync(patientMedication.Id);
            var dto = MapToDto(createdMedication!);

            return new ApiResponse<PatientMedicationDto>
            {
                Success = true,
                Message = "Medication prescribed successfully",
                Data = dto
            };
        }

        public async Task<ApiResponse<PatientMedicationDto>> UpdatePatientMedicationAsync(int id, UpdatePatientMedicationRequest request)
        {
            var patientMedication = await _uow.PatientMedications.GetWithMedicationAsync(id);
            if (patientMedication == null)
            {
                return new ApiResponse<PatientMedicationDto>
                {
                    Success = false,
                    Message = "Patient medication not found"
                };
            }

            // Update properties
            patientMedication.EndDate = request.EndDate;
            patientMedication.Dosage = request.Dosage;
            patientMedication.Frequency = request.Frequency;
            patientMedication.Instructions = request.Instructions;
            patientMedication.Status = request.Status;
            patientMedication.DiscontinuationReason = request.DiscontinuationReason;
            patientMedication.DiscontinuationDate = request.DiscontinuationDate;
            patientMedication.Notes = request.Notes;
            patientMedication.IsCurrentlyTaking = request.IsCurrentlyTaking;
            patientMedication.RefillsRemaining = request.RefillsRemaining;
            patientMedication.LastRefillDate = request.LastRefillDate;
            patientMedication.NextRefillDue = request.NextRefillDue;
            patientMedication.UpdatedAt = DateTime.UtcNow;

            var result = await _uow.PatientMedications.UpdateAsync(patientMedication);
            if (!result.Success)
            {
                return new ApiResponse<PatientMedicationDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var dto = MapToDto(patientMedication);
            return new ApiResponse<PatientMedicationDto>
            {
                Success = true,
                Message = "Patient medication updated successfully",
                Data = dto
            };
        }

        public async Task<ApiResponse<PatientMedicationDto>> DiscontinueMedicationAsync(int id, string reason)
        {
            var patientMedication = await _uow.PatientMedications.GetWithMedicationAsync(id);
            if (patientMedication == null)
            {
                return new ApiResponse<PatientMedicationDto>
                {
                    Success = false,
                    Message = "Patient medication not found"
                };
            }

            patientMedication.Status = "Discontinued";
            patientMedication.IsCurrentlyTaking = false;
            patientMedication.DiscontinuationReason = reason;
            patientMedication.DiscontinuationDate = DateTime.UtcNow;
            patientMedication.UpdatedAt = DateTime.UtcNow;

            var result = await _uow.PatientMedications.UpdateAsync(patientMedication);
            if (!result.Success)
            {
                return new ApiResponse<PatientMedicationDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var dto = MapToDto(patientMedication);
            return new ApiResponse<PatientMedicationDto>
            {
                Success = true,
                Message = "Medication discontinued successfully",
                Data = dto
            };
        }

        public async Task<ApiResponse<IEnumerable<PatientMedicationDto>>> GetFilteredPatientMedicationsAsync(PatientMedicationFilterRequest filter)
        {
            var patientMedications = await _uow.PatientMedications.GetFilteredPatientMedicationsAsync(
                filter.PatientId,
                filter.MedicationId,
                filter.PrescribedByDoctorId,
                filter.Status,
                filter.IsCurrentlyTaking,
                filter.StartDateFrom,
                filter.StartDateTo,
                filter.Category,
                filter.MedicationType,
                filter.PageNumber,
                filter.PageSize);

            var dtos = patientMedications.Select(MapToDto);

            return new ApiResponse<IEnumerable<PatientMedicationDto>>
            {
                Success = true,
                Data = dtos
            };
        }

        private static PatientMedicationDto MapToDto(PatientMedication patientMedication)
        {
            return new PatientMedicationDto
            {
                Id = patientMedication.Id,
                PatientId = patientMedication.PatientId,
                MedicationId = patientMedication.MedicationId,
                PrescribedByDoctorId = patientMedication.PrescribedByDoctorId,
                PrescribedDate = patientMedication.PrescribedDate,
                StartDate = patientMedication.StartDate,
                EndDate = patientMedication.EndDate,
                Dosage = patientMedication.Dosage,
                Frequency = patientMedication.Frequency,
                Instructions = patientMedication.Instructions,
                Status = patientMedication.Status,
                DiscontinuationReason = patientMedication.DiscontinuationReason,
                DiscontinuationDate = patientMedication.DiscontinuationDate,
                Notes = patientMedication.Notes,
                IsCurrentlyTaking = patientMedication.IsCurrentlyTaking,
                RefillsRemaining = patientMedication.RefillsRemaining,
                LastRefillDate = patientMedication.LastRefillDate,
                NextRefillDue = patientMedication.NextRefillDue,
                AdherencePercentage = patientMedication.AdherencePercentage,
                MissedDoses = patientMedication.MissedDoses,
                LastAdherenceUpdate = patientMedication.LastAdherenceUpdate,
                CreatedAt = patientMedication.CreatedAt,
                UpdatedAt = patientMedication.UpdatedAt,
                Medication = patientMedication.Medication != null ? new MedicationDto
                {
                    Id = patientMedication.Medication.Id,
                    Name = patientMedication.Medication.Name,
                    GenericName = patientMedication.Medication.GenericName,
                    BrandName = patientMedication.Medication.BrandName,
                    Category = patientMedication.Medication.Category,
                    MedicationType = patientMedication.Medication.MedicationType,
                    Strength = patientMedication.Medication.Strength,
                    Form = patientMedication.Medication.Form,
                    Description = patientMedication.Medication.Description
                } : null,
                PatientName = patientMedication.Patient?.FullName,
                PatientCode = patientMedication.Patient?.PatientCode
            };
        }
    }
}
