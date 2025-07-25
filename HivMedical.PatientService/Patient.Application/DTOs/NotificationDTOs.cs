namespace Patient.Application.DTOs
{
    public class SendEmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    public class NotificationTemplateDto
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Language { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateNotificationTemplateRequest
    {
        public string TemplateName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string NotificationType { get; set; } = "Email";
        public string Language { get; set; } = "vi-VN";
        public string Parameters { get; set; } = "{}";
    }

    public class UpdateNotificationTemplateRequest
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Parameters { get; set; } = "{}";
    }

    public class NotificationLogDto
    {
        public int Id { get; set; }
        public string RecipientId { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class CreateNotificationLogRequest
    {
        public string RecipientId { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EventData { get; set; } = string.Empty;
    }

    public class NotificationApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }

    public class NotificationApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
