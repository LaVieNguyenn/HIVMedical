using Patient.Application.DTOs;
using Patient.Domain.Entities;
using Patient.Infrastructure.UnitOfWorks;
using SharedLibrary.Response;

namespace Patient.Application.Services
{
    public class MedicationService
    {
        private readonly IUnitOfWork _uow;

        public MedicationService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<ApiResponse<MedicationDto>> GetMedicationByIdAsync(int id)
        {
            var medication = await _uow.Medications.GetByIdAsync(id);
            if (medication == null)
            {
                return new ApiResponse<MedicationDto>
                {
                    Success = false,
                    Message = "Medication not found"
                };
            }

            var medicationDto = MapToDto(medication);
            return new ApiResponse<MedicationDto>
            {
                Success = true,
                Data = medicationDto
            };
        }

        public async Task<ApiResponse<IEnumerable<MedicationDto>>> GetAllMedicationsAsync()
        {
            var medications = await _uow.Medications.GetActiveAsync();
            var medicationDtos = medications.Select(MapToDto);

            return new ApiResponse<IEnumerable<MedicationDto>>
            {
                Success = true,
                Data = medicationDtos
            };
        }

        public async Task<ApiResponse<IEnumerable<MedicationDto>>> GetHIVMedicationsAsync()
        {
            var medications = await _uow.Medications.GetHIVMedicationsAsync();
            var medicationDtos = medications.Select(MapToDto);

            return new ApiResponse<IEnumerable<MedicationDto>>
            {
                Success = true,
                Data = medicationDtos
            };
        }

        public async Task<ApiResponse<IEnumerable<MedicationDto>>> GetARVMedicationsAsync()
        {
            var medications = await _uow.Medications.GetARVMedicationsAsync();
            var medicationDtos = medications.Select(MapToDto);

            return new ApiResponse<IEnumerable<MedicationDto>>
            {
                Success = true,
                Data = medicationDtos
            };
        }

        public async Task<ApiResponse<IEnumerable<MedicationDto>>> SearchMedicationsAsync(string searchTerm)
        {
            var medications = await _uow.Medications.SearchByNameOrGenericAsync(searchTerm);
            var medicationDtos = medications.Select(MapToDto);

            return new ApiResponse<IEnumerable<MedicationDto>>
            {
                Success = true,
                Data = medicationDtos
            };
        }

        public async Task<ApiResponse<IEnumerable<MedicationDto>>> GetFilteredMedicationsAsync(MedicationFilterRequest filter)
        {
            var medications = await _uow.Medications.GetFilteredMedicationsAsync(
                filter.Name,
                filter.Category,
                filter.MedicationType,
                filter.Form,
                filter.IsActive,
                filter.PageNumber,
                filter.PageSize);
            var medicationDtos = medications.Select(MapToDto);

            return new ApiResponse<IEnumerable<MedicationDto>>
            {
                Success = true,
                Data = medicationDtos
            };
        }

        public async Task<ApiResponse<MedicationDto>> CreateMedicationAsync(CreateMedicationRequest request)
        {
            // Check if medication already exists
            if (await _uow.Medications.ExistsByNameAsync(request.Name))
            {
                return new ApiResponse<MedicationDto>
                {
                    Success = false,
                    Message = "Medication with this name already exists"
                };
            }

            var medication = new Medication
            {
                Name = request.Name,
                GenericName = request.GenericName,
                BrandName = request.BrandName,
                Category = request.Category,
                MedicationType = request.MedicationType,
                Strength = request.Strength,
                Form = request.Form,
                Description = request.Description,
                SideEffects = request.SideEffects,
                Contraindications = request.Contraindications,
                DrugInteractions = request.DrugInteractions,
                StorageInstructions = request.StorageInstructions,
                RequiresPrescription = request.RequiresPrescription,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _uow.Medications.CreateAsync(medication);
            if (!result.Success)
            {
                return new ApiResponse<MedicationDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var medicationDto = MapToDto(medication);
            return new ApiResponse<MedicationDto>
            {
                Success = true,
                Message = "Medication created successfully",
                Data = medicationDto
            };
        }

        public async Task<ApiResponse<MedicationDto>> UpdateMedicationAsync(int id, UpdateMedicationRequest request)
        {
            var medication = await _uow.Medications.GetByIdAsync(id);
            if (medication == null)
            {
                return new ApiResponse<MedicationDto>
                {
                    Success = false,
                    Message = "Medication not found"
                };
            }

            // Check if name is being changed and if new name already exists
            if (medication.Name != request.Name && await _uow.Medications.ExistsByNameAsync(request.Name))
            {
                return new ApiResponse<MedicationDto>
                {
                    Success = false,
                    Message = "Medication with this name already exists"
                };
            }

            // Update medication properties
            medication.Name = request.Name;
            medication.GenericName = request.GenericName;
            medication.BrandName = request.BrandName;
            medication.Category = request.Category;
            medication.MedicationType = request.MedicationType;
            medication.Strength = request.Strength;
            medication.Form = request.Form;
            medication.Description = request.Description;
            medication.SideEffects = request.SideEffects;
            medication.Contraindications = request.Contraindications;
            medication.DrugInteractions = request.DrugInteractions;
            medication.StorageInstructions = request.StorageInstructions;
            medication.RequiresPrescription = request.RequiresPrescription;
            medication.UpdatedAt = DateTime.UtcNow;

            var result = await _uow.Medications.UpdateAsync(medication);
            if (!result.Success)
            {
                return new ApiResponse<MedicationDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var medicationDto = MapToDto(medication);
            return new ApiResponse<MedicationDto>
            {
                Success = true,
                Message = "Medication updated successfully",
                Data = medicationDto
            };
        }

        public async Task<ApiResponse<bool>> DeleteMedicationAsync(int id)
        {
            var medication = await _uow.Medications.GetByIdAsync(id);
            if (medication == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Medication not found"
                };
            }

            // Soft delete - just mark as inactive
            medication.IsActive = false;
            medication.UpdatedAt = DateTime.UtcNow;

            var result = await _uow.Medications.UpdateAsync(medication);
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
                Message = "Medication deactivated successfully",
                Data = true
            };
        }

        private static MedicationDto MapToDto(Medication medication)
        {
            return new MedicationDto
            {
                Id = medication.Id,
                Name = medication.Name,
                GenericName = medication.GenericName,
                BrandName = medication.BrandName,
                Category = medication.Category,
                MedicationType = medication.MedicationType,
                Strength = medication.Strength,
                Form = medication.Form,
                Description = medication.Description,
                SideEffects = medication.SideEffects,
                Contraindications = medication.Contraindications,
                DrugInteractions = medication.DrugInteractions,
                StorageInstructions = medication.StorageInstructions,
                IsActive = medication.IsActive,
                RequiresPrescription = medication.RequiresPrescription,
                CreatedAt = medication.CreatedAt,
                UpdatedAt = medication.UpdatedAt
            };
        }
    }
}


