using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class Medication : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // ARV, Opportunistic Infection, Supplement
        public string MedicationType { get; set; } = string.Empty; // NRTI, NNRTI, PI, INSTI, etc.
        public string Strength { get; set; } = string.Empty; // e.g., "600mg", "200mg/25mg"
        public string Form { get; set; } = string.Empty; // Tablet, Capsule, Syrup, Injection
        public string? Description { get; set; }
        public string? SideEffects { get; set; }
        public string? Contraindications { get; set; }
        public string? DrugInteractions { get; set; }
        public string? StorageInstructions { get; set; }
        public bool IsActive { get; set; } = true;
        public bool RequiresPrescription { get; set; } = true;
        
        // Navigation properties
        public ICollection<PatientMedication> PatientMedications { get; set; } = new List<PatientMedication>();
        public ICollection<MedicationSchedule> MedicationSchedules { get; set; } = new List<MedicationSchedule>();
    }
}
