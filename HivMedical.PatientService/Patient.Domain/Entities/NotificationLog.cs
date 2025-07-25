using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class NotificationLog : BaseEntity
    {
        public string RecipientId { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string RecipientPhone { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pending, Sent, Failed
        public string EventType { get; set; } = string.Empty; // AppointmentCreated, MedicationPrescribed, etc.
        public int RetryCount { get; set; } = 0;
        public DateTime? SentAt { get; set; }
        public string? ErrorMessage { get; set; }
        public string EventData { get; set; } = "{}"; // Original event JSON
    }
}
