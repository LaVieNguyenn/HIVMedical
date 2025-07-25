using Microsoft.Extensions.Logging;
using Patient.Application.Services.Interfaces;
using Patient.Domain.Entities;
using Patient.Application.DTOs;

namespace Patient.Application.Services
{
    public class NotificationLogService : INotificationLogService
    {
        private readonly ILogger<NotificationLogService> _logger;
        
        // For demo purposes, using in-memory storage
        private readonly List<NotificationLog> _logs;
        private int _nextId = 1;

        public NotificationLogService(ILogger<NotificationLogService> logger)
        {
            _logger = logger;
            _logs = new List<NotificationLog>();
        }

        public async Task<NotificationLogDto> CreateNotificationLogAsync(CreateNotificationLogRequest request)
        {
            await Task.CompletedTask;
            
            try
            {
                var log = new NotificationLog
                {
                    Id = _nextId++,
                    RecipientId = request.RecipientId,
                    RecipientEmail = request.RecipientEmail,
                    EventType = request.EventType,
                    NotificationType = request.NotificationType,
                    Status = request.Status,
                    EventData = request.EventData,
                    CreatedAt = DateTime.UtcNow
                };

                _logs.Add(log);

                _logger.LogInformation("Created notification log {LogId} for recipient {RecipientId}", 
                    log.Id, request.RecipientId);

                return MapToDto(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification log");
                throw;
            }
        }

        public async Task UpdateNotificationLogStatusAsync(int logId, string status, string? errorMessage)
        {
            await Task.CompletedTask;
            
            try
            {
                var log = _logs.FirstOrDefault(l => l.Id == logId);
                if (log != null)
                {
                    log.Status = status;
                    log.ErrorMessage = errorMessage;
                    
                    if (status == "Sent")
                    {
                        log.SentAt = DateTime.UtcNow;
                    }

                    _logger.LogInformation("Updated notification log {LogId} status to {Status}", 
                        logId, status);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification log status");
                throw;
            }
        }

        public async Task<NotificationApiResponse<IEnumerable<NotificationLogDto>>> GetNotificationLogsByRecipientAsync(string recipientId)
        {
            await Task.CompletedTask;
            
            try
            {
                var logs = _logs
                    .Where(l => l.RecipientId == recipientId)
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(MapToDto);

                return new NotificationApiResponse<IEnumerable<NotificationLogDto>>
                {
                    Success = true,
                    Data = logs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification logs by recipient");
                return new NotificationApiResponse<IEnumerable<NotificationLogDto>>
                {
                    Success = false,
                    Message = "Failed to retrieve notification logs"
                };
            }
        }

        public async Task<NotificationApiResponse<IEnumerable<NotificationLogDto>>> GetNotificationLogsByEventTypeAsync(string eventType)
        {
            await Task.CompletedTask;
            
            try
            {
                var logs = _logs
                    .Where(l => l.EventType == eventType)
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(MapToDto);

                return new NotificationApiResponse<IEnumerable<NotificationLogDto>>
                {
                    Success = true,
                    Data = logs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification logs by event type");
                return new NotificationApiResponse<IEnumerable<NotificationLogDto>>
                {
                    Success = false,
                    Message = "Failed to retrieve notification logs"
                };
            }
        }

        public async Task<NotificationApiResponse<NotificationLogDto>> GetNotificationLogByIdAsync(int id)
        {
            await Task.CompletedTask;
            
            try
            {
                var log = _logs.FirstOrDefault(l => l.Id == id);
                if (log == null)
                {
                    return new NotificationApiResponse<NotificationLogDto>
                    {
                        Success = false,
                        Message = "Notification log not found"
                    };
                }

                return new NotificationApiResponse<NotificationLogDto>
                {
                    Success = true,
                    Data = MapToDto(log)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification log by ID");
                return new NotificationApiResponse<NotificationLogDto>
                {
                    Success = false,
                    Message = "Failed to retrieve notification log"
                };
            }
        }

        private NotificationLogDto MapToDto(NotificationLog log)
        {
            return new NotificationLogDto
            {
                Id = log.Id,
                RecipientId = log.RecipientId,
                RecipientEmail = log.RecipientEmail,
                Subject = log.Subject,
                NotificationType = log.NotificationType,
                Status = log.Status,
                EventType = log.EventType,
                CreatedAt = log.CreatedAt,
                SentAt = log.SentAt,
                ErrorMessage = log.ErrorMessage
            };
        }
    }
}

