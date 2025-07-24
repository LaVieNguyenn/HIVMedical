using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; } // Reference to doctor
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty; // Consultation, Lab Test, Follow-up
        public string Status { get; set; } = string.Empty; // Scheduled, Completed, Cancelled, No-Show
        public string? Notes { get; set; }
        public string? Reason { get; set; }
        
        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
    }
}
