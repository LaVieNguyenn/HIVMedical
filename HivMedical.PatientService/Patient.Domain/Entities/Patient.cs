using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public string PatientCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public byte Gender { get; set; } // 0: Female, 1: Male, 2: Other
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public bool IsActive { get; set; } = true;
        public int AuthUserId { get; set; } // Reference to User from Auth service
        
        // HIV specific fields
        public DateTime? DiagnosisDate { get; set; }
        public string? HIVStatus { get; set; } // Positive, Negative, Unknown
        public string? TreatmentStatus { get; set; } // On Treatment, Not on Treatment, Lost to Follow-up
        
        // Navigation properties
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<PatientMedication> PatientMedications { get; set; } = new List<PatientMedication>();
        public ICollection<MedicationSchedule> MedicationSchedules { get; set; } = new List<MedicationSchedule>();
        public ICollection<MedicationAdherence> MedicationAdherences { get; set; } = new List<MedicationAdherence>();
    }
}
