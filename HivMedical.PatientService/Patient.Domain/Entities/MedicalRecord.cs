using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class MedicalRecord : BaseEntity
    {
        public int PatientId { get; set; }
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; } = string.Empty; // Lab Result, Clinical Note, Treatment Update
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? LabResults { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }
        public int DoctorId { get; set; } 
        
        // Navigation properties
        public Patient Patient { get; set; } = null!;
    }
}
