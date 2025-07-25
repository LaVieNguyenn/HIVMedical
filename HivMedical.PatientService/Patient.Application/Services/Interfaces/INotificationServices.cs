using Patient.Application.DTOs;

namespace Patient.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task<NotificationApiResponse> SendEmailAsync(SendEmailRequest request);
        Task<NotificationApiResponse> SendBulkEmailAsync(List<SendEmailRequest> requests);
    }
    
    public interface INotificationTemplateService
    {
        Task<NotificationTemplateDto?> GetTemplateByNameAsync(string templateName);
        Task<NotificationApiResponse<NotificationTemplateDto>> CreateTemplateAsync(CreateNotificationTemplateRequest request);
        Task<NotificationApiResponse<NotificationTemplateDto>> UpdateTemplateAsync(int id, UpdateNotificationTemplateRequest request);
        Task<NotificationApiResponse> DeleteTemplateAsync(int id);
        Task<NotificationApiResponse<IEnumerable<NotificationTemplateDto>>> GetAllTemplatesAsync();
    }
    
    public interface INotificationLogService
    {
        Task<NotificationLogDto> CreateNotificationLogAsync(CreateNotificationLogRequest request);
        Task UpdateNotificationLogStatusAsync(int logId, string status, string? errorMessage);
        Task<NotificationApiResponse<IEnumerable<NotificationLogDto>>> GetNotificationLogsByRecipientAsync(string recipientId);
        Task<NotificationApiResponse<IEnumerable<NotificationLogDto>>> GetNotificationLogsByEventTypeAsync(string eventType);
        Task<NotificationApiResponse<NotificationLogDto>> GetNotificationLogByIdAsync(int id);
    }
}
