using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class NotificationTemplate : BaseEntity
    {
        public string TemplateName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty; // Email, SMS, Push
        public bool IsActive { get; set; } = true;
        public string Language { get; set; } = "vi-VN";
        
        // Template parameters (stored as JSON)
        public string Parameters { get; set; } = "{}";
    }
}
