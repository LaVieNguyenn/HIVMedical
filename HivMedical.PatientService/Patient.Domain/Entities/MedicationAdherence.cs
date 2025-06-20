using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class MedicationAdherence : BaseEntity
    {
        public int PatientMedicationId { get; set; }
        public int PatientId { get; set; }
        public int MedicationId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public DateTime? ActualTakenDateTime { get; set; }
        public string Status { get; set; } = string.Empty; // Taken, Missed, Late, Skipped
        public string? Reason { get; set; } // Reason for missing/skipping
        public string? Notes { get; set; }
        public bool WasReminded { get; set; } = false;
        public DateTime? ReminderSentAt { get; set; }
        public string? SideEffectsReported { get; set; }
        public int? DelayMinutes { get; set; } // How many minutes late if taken late
        
        // Navigation properties
        public PatientMedication PatientMedication { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
        public Medication Medication { get; set; } = null!;
    }
}
