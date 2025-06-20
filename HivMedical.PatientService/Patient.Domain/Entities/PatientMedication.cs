using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class PatientMedication : BaseEntity
    {
        public int PatientId { get; set; }
        public int MedicationId { get; set; }
        public int PrescribedByDoctorId { get; set; } // Reference to doctor from Auth service
        public DateTime PrescribedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Dosage { get; set; } = string.Empty; // e.g., "1 tablet", "2 capsules"
        public string Frequency { get; set; } = string.Empty; // e.g., "Once daily", "Twice daily", "Every 8 hours"
        public string? Instructions { get; set; } // e.g., "Take with food", "Take on empty stomach"
        public string Status { get; set; } = string.Empty; // Active, Completed, Discontinued, Paused
        public string? DiscontinuationReason { get; set; }
        public DateTime? DiscontinuationDate { get; set; }
        public string? Notes { get; set; }
        public bool IsCurrentlyTaking { get; set; } = true;
        public int? RefillsRemaining { get; set; }
        public DateTime? LastRefillDate { get; set; }
        public DateTime? NextRefillDue { get; set; }
        
        // Adherence tracking
        public decimal? AdherencePercentage { get; set; }
        public int? MissedDoses { get; set; }
        public DateTime? LastAdherenceUpdate { get; set; }
        
        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Medication Medication { get; set; } = null!;
        public ICollection<MedicationSchedule> MedicationSchedules { get; set; } = new List<MedicationSchedule>();
        public ICollection<MedicationAdherence> MedicationAdherences { get; set; } = new List<MedicationAdherence>();
    }
}
