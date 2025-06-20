using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class MedicationSchedule : BaseEntity
    {
        public int PatientMedicationId { get; set; }
        public int MedicationId { get; set; }
        public int PatientId { get; set; }
        public TimeSpan ScheduledTime { get; set; } // e.g., 08:00:00, 20:00:00
        public string DayOfWeek { get; set; } = string.Empty; // Daily, Monday, Tuesday, etc.
        public string? SpecialInstructions { get; set; }
        public bool IsActive { get; set; } = true;
        public bool ReminderEnabled { get; set; } = true;
        public int? ReminderMinutesBefore { get; set; } = 30; // Remind 30 minutes before
        
        // Navigation properties
        public PatientMedication PatientMedication { get; set; } = null!;
        public Medication Medication { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
    }
}
