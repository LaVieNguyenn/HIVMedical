namespace Patient.Application.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Additional info for display
        public string? PatientName { get; set; }
        public string? PatientCode { get; set; }
    }

    public class CreateAppointmentRequest
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty; // Consultation, Lab Test, Follow-up
        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentRequest
    {
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentStatusRequest
    {
        public string Status { get; set; } = string.Empty; // Scheduled, Completed, Cancelled, No-Show
        public string? Notes { get; set; }
    }

    public class AppointmentFilterRequest
    {
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
        public string? AppointmentType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AppointmentSummaryDto
    {
        public int TotalAppointments { get; set; }
        public int ScheduledAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public AppointmentDto? NextAppointment { get; set; }
    }
}
