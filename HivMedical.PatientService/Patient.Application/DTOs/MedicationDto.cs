namespace Patient.Application.DTOs
{
    public class MedicationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MedicationType { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SideEffects { get; set; }
        public string? Contraindications { get; set; }
        public string? DrugInteractions { get; set; }
        public string? StorageInstructions { get; set; }
        public bool IsActive { get; set; }
        public bool RequiresPrescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMedicationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MedicationType { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SideEffects { get; set; }
        public string? Contraindications { get; set; }
        public string? DrugInteractions { get; set; }
        public string? StorageInstructions { get; set; }
        public bool RequiresPrescription { get; set; } = true;
    }

    public class UpdateMedicationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MedicationType { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SideEffects { get; set; }
        public string? Contraindications { get; set; }
        public string? DrugInteractions { get; set; }
        public string? StorageInstructions { get; set; }
        public bool RequiresPrescription { get; set; }
    }

    public class MedicationFilterRequest
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? MedicationType { get; set; }
        public string? Form { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
