namespace Patient.Application.Events
{
    public class AppointmentCreatedEvent
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientCode { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public AppointmentCreatedEvent(int appointmentId, int patientId, string patientName, string patientCode,
            int doctorId, DateTime appointmentDate, TimeSpan appointmentTime, string appointmentType,
            string? reason, string? notes, DateTime createdAt)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            PatientName = patientName;
            PatientCode = patientCode;
            DoctorId = doctorId;
            AppointmentDate = appointmentDate;
            AppointmentTime = appointmentTime;
            AppointmentType = appointmentType;
            Reason = reason;
            Notes = notes;
            CreatedAt = createdAt;
        }
    }

    public class AppointmentStatusChangedEvent
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime UpdatedAt { get; set; }

        public AppointmentStatusChangedEvent(int appointmentId, int patientId, string patientName, int doctorId,
            DateTime appointmentDate, TimeSpan appointmentTime, string appointmentType, string oldStatus,
            string newStatus, string? notes, DateTime updatedAt)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            PatientName = patientName;
            DoctorId = doctorId;
            AppointmentDate = appointmentDate;
            AppointmentTime = appointmentTime;
            AppointmentType = appointmentType;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            Notes = notes;
            UpdatedAt = updatedAt;
        }
    }

    public class AppointmentCancelledEvent
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
        public DateTime CancelledAt { get; set; }

        public AppointmentCancelledEvent(int appointmentId, int patientId, string patientName, int doctorId,
            DateTime appointmentDate, TimeSpan appointmentTime, string appointmentType, string? cancellationReason, DateTime cancelledAt)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            PatientName = patientName;
            DoctorId = doctorId;
            AppointmentDate = appointmentDate;
            AppointmentTime = appointmentTime;
            AppointmentType = appointmentType;
            CancellationReason = cancellationReason;
            CancelledAt = cancelledAt;
        }
    }
}
