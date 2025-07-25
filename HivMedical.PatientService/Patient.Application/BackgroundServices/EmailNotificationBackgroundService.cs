using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedLibrary.Messaging;
using Patient.Application.Events;
using Patient.Application.Services.Interfaces;
using Patient.Application.DTOs;
using System.Text.Json;
using SharedLibrary.Messaging.Events;

namespace Patient.Application.BackgroundServices
{
    public class EmailNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailNotificationBackgroundService> _logger;
        private readonly IEventBus _eventBus;

        public EmailNotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<EmailNotificationBackgroundService> logger,
            IEventBus eventBus)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _eventBus = eventBus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Notification Background Service started");

            // Subscribe to events
            _eventBus.Subscribe<UserRegisteredEvent, UserRegisteredEventHandler>();
            _eventBus.Subscribe<PatientCreatedEvent, PatientCreatedEventHandler>();
            _eventBus.Subscribe<PatientUpdatedEvent, PatientUpdatedEventHandler>();
            _eventBus.Subscribe<AppointmentCreatedEvent, AppointmentCreatedEventHandler>();
            _eventBus.Subscribe<MedicationPrescribedEvent, MedicationPrescribedEventHandler>();
            _eventBus.Subscribe<MedicationRefillDueEvent, MedicationRefillDueEventHandler>();

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("Email Notification Background Service stopped");
        }
    }

    // UserRegisteredEvent Handler
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserRegisteredEventHandler> _logger;

        public UserRegisteredEventHandler(
            IServiceProvider serviceProvider,
            ILogger<UserRegisteredEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(UserRegisteredEvent @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var notificationLogService = scope.ServiceProvider.GetRequiredService<INotificationLogService>();

            try
            {
                _logger.LogInformation("Processing UserRegisteredEvent for User: {FullName}", @event.FullName);

                // Create notification log entry
                var notificationLog = await notificationLogService.CreateNotificationLogAsync(new CreateNotificationLogRequest
                {
                    RecipientId = @event.UserId.ToString(),
                    RecipientEmail = @event.Email,
                    EventType = "UserRegistered",
                    NotificationType = "Email",
                    Status = "Pending",
                    EventData = JsonSerializer.Serialize(@event)
                });

                // Send welcome email
                var emailRequest = new SendEmailRequest
                {
                    To = @event.Email,
                    Subject = "Chào mừng bạn đến với HIV Medical Center",
                    TemplateName = "PatientCreated",
                    Parameters = new Dictionary<string, object>
                    {
                        ["PatientName"] = @event.FullName,
                        ["PatientCode"] = "TEMP-" + @event.UserId,
                        ["Email"] = @event.Email,
                        ["CreatedDate"] = DateTime.UtcNow.ToString("dd/MM/yyyy")
                    }
                };

                var result = await emailService.SendEmailAsync(emailRequest);

                // Update notification log
                await notificationLogService.UpdateNotificationLogStatusAsync(notificationLog.Id, 
                    result.Success ? "Sent" : "Failed", 
                    result.Success ? null : result.Message);

                _logger.LogInformation("User registration email sent to {Email}: {Success}", 
                    @event.Email, result.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing UserRegisteredEvent for User: {FullName}", @event.FullName);
            }
        }
    }

    // PatientCreatedEvent Handler
    public class PatientCreatedEventHandler : IEventHandler<PatientCreatedEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatientCreatedEventHandler> _logger;

        public PatientCreatedEventHandler(
            IServiceProvider serviceProvider,
            ILogger<PatientCreatedEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(PatientCreatedEvent @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var notificationLogService = scope.ServiceProvider.GetRequiredService<INotificationLogService>();

            try
            {
                _logger.LogInformation("Processing PatientCreatedEvent for Patient: {PatientName}", @event.FullName);

                if (string.IsNullOrEmpty(@event.Email))
                {
                    _logger.LogWarning("No email found for Patient: {PatientName}", @event.FullName);
                    return;
                }

                // Create notification log entry
                var notificationLog = await notificationLogService.CreateNotificationLogAsync(new CreateNotificationLogRequest
                {
                    RecipientId = @event.PatientId.ToString(),
                    RecipientEmail = @event.Email,
                    EventType = "PatientCreated",
                    NotificationType = "Email",
                    Status = "Pending",
                    EventData = JsonSerializer.Serialize(@event)
                });

                // Send patient welcome email
                var emailRequest = new SendEmailRequest
                {
                    To = @event.Email,
                    Subject = "Chào mừng bạn đến với HIV Medical Center",
                    TemplateName = "PatientCreated",
                    Parameters = new Dictionary<string, object>
                    {
                        ["PatientName"] = @event.FullName,
                        ["PatientCode"] = @event.PatientCode,
                        ["Email"] = @event.Email,
                        ["CreatedDate"] = @event.CreatedAt.ToString("dd/MM/yyyy")
                    }
                };

                var result = await emailService.SendEmailAsync(emailRequest);

                // Update notification log
                await notificationLogService.UpdateNotificationLogStatusAsync(notificationLog.Id, 
                    result.Success ? "Sent" : "Failed", 
                    result.Success ? null : result.Message);

                _logger.LogInformation("Patient welcome email sent to {Email}: {Success}", 
                    @event.Email, result.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientCreatedEvent for Patient: {PatientName}", @event.FullName);
            }
        }
    }

    // PatientUpdatedEvent Handler
    public class PatientUpdatedEventHandler : IEventHandler<PatientUpdatedEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PatientUpdatedEventHandler> _logger;

        public PatientUpdatedEventHandler(
            IServiceProvider serviceProvider,
            ILogger<PatientUpdatedEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(PatientUpdatedEvent @event)
        {
            // For now, we don't send email for patient updates
            // But we log it for auditing purposes
            _logger.LogInformation("Patient {PatientName} updated information", @event.FullName);
            await Task.CompletedTask;
        }
    }

    // AppointmentCreatedEvent Handler
    public class AppointmentCreatedEventHandler : IEventHandler<AppointmentCreatedEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentCreatedEventHandler> _logger;

        public AppointmentCreatedEventHandler(
            IServiceProvider serviceProvider,
            ILogger<AppointmentCreatedEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(AppointmentCreatedEvent @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var notificationLogService = scope.ServiceProvider.GetRequiredService<INotificationLogService>();

            try
            {
                _logger.LogInformation("Processing AppointmentCreatedEvent for Patient: {PatientName}", @event.PatientName);

                // For now, use a placeholder email since we don't have patient email in the event
                var patientEmail = await GetPatientEmail(@event.PatientId);
                
                if (string.IsNullOrEmpty(patientEmail))
                {
                    _logger.LogWarning("No email found for Patient ID: {PatientId}", @event.PatientId);
                    return;
                }

                // Create notification log entry
                var notificationLog = await notificationLogService.CreateNotificationLogAsync(new CreateNotificationLogRequest
                {
                    RecipientId = @event.PatientId.ToString(),
                    RecipientEmail = patientEmail,
                    EventType = "AppointmentCreated",
                    NotificationType = "Email",
                    Status = "Pending",
                    EventData = JsonSerializer.Serialize(@event)
                });

                // Send appointment confirmation email
                var emailRequest = new SendEmailRequest
                {
                    To = patientEmail,
                    Subject = "Xác nhận lịch hẹn khám - HIV Medical Center",
                    TemplateName = "AppointmentConfirmation",
                    Parameters = new Dictionary<string, object>
                    {
                        ["PatientName"] = @event.PatientName,
                        ["AppointmentDate"] = @event.AppointmentDate.ToString("dd/MM/yyyy"),
                        ["AppointmentTime"] = @event.AppointmentTime.ToString(@"hh\:mm"),
                        ["AppointmentType"] = @event.AppointmentType,
                        ["Reason"] = @event.Reason ?? "",
                        ["Notes"] = @event.Notes ?? ""
                    }
                };

                var result = await emailService.SendEmailAsync(emailRequest);

                // Update notification log
                await notificationLogService.UpdateNotificationLogStatusAsync(notificationLog.Id, 
                    result.Success ? "Sent" : "Failed", 
                    result.Success ? null : result.Message);

                _logger.LogInformation("Appointment confirmation email sent to {Email}: {Success}", 
                    patientEmail, result.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AppointmentCreatedEvent for Patient: {PatientName}", @event.PatientName);
            }
        }

        private async Task<string> GetPatientEmail(int patientId)
        {
            // TODO: Implement actual patient email retrieval
            // For now, return a placeholder
            await Task.CompletedTask;
            return "patient@example.com";
        }
    }

    // MedicationPrescribedEvent Handler
    public class MedicationPrescribedEventHandler : IEventHandler<MedicationPrescribedEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MedicationPrescribedEventHandler> _logger;

        public MedicationPrescribedEventHandler(
            IServiceProvider serviceProvider,
            ILogger<MedicationPrescribedEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(MedicationPrescribedEvent @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var notificationLogService = scope.ServiceProvider.GetRequiredService<INotificationLogService>();

            try
            {
                _logger.LogInformation("Processing MedicationPrescribedEvent for Patient: {PatientName}", @event.PatientName);

                var patientEmail = await GetPatientEmail(@event.PatientId);
                
                if (string.IsNullOrEmpty(patientEmail))
                {
                    _logger.LogWarning("No email found for Patient ID: {PatientId}", @event.PatientId);
                    return;
                }

                // Create notification log entry
                var notificationLog = await notificationLogService.CreateNotificationLogAsync(new CreateNotificationLogRequest
                {
                    RecipientId = @event.PatientId.ToString(),
                    RecipientEmail = patientEmail,
                    EventType = "MedicationPrescribed",
                    NotificationType = "Email",
                    Status = "Pending",
                    EventData = JsonSerializer.Serialize(@event)
                });

                // Send medication prescribed email
                var emailRequest = new SendEmailRequest
                {
                    To = patientEmail,
                    Subject = "Thông tin đơn thuốc mới - HIV Medical Center",
                    TemplateName = "MedicationPrescribed",
                    Parameters = new Dictionary<string, object>
                    {
                        ["PatientName"] = @event.PatientName,
                        ["MedicationName"] = @event.MedicationName,
                        ["Dosage"] = @event.Dosage,
                        ["Frequency"] = @event.Frequency,
                        ["Instructions"] = @event.Instructions ?? "",
                        ["StartDate"] = @event.StartDate.ToString("dd/MM/yyyy"),
                        ["Notes"] = @event.Notes ?? ""
                    }
                };

                var result = await emailService.SendEmailAsync(emailRequest);

                // Update notification log
                await notificationLogService.UpdateNotificationLogStatusAsync(notificationLog.Id, 
                    result.Success ? "Sent" : "Failed", 
                    result.Success ? null : result.Message);

                _logger.LogInformation("Medication prescribed email sent to {Email}: {Success}", 
                    patientEmail, result.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MedicationPrescribedEvent for Patient: {PatientName}", @event.PatientName);
            }
        }

        private async Task<string> GetPatientEmail(int patientId)
        {
           
            await Task.CompletedTask;
            return "patient@example.com";
        }
    }

    // MedicationRefillDueEvent Handler
    public class MedicationRefillDueEventHandler : IEventHandler<MedicationRefillDueEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MedicationRefillDueEventHandler> _logger;

        public MedicationRefillDueEventHandler(
            IServiceProvider serviceProvider,
            ILogger<MedicationRefillDueEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(MedicationRefillDueEvent @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var notificationLogService = scope.ServiceProvider.GetRequiredService<INotificationLogService>();

            try
            {
                _logger.LogInformation("Processing MedicationRefillDueEvent for Patient: {PatientName}", @event.PatientName);

                var patientEmail = await GetPatientEmail(@event.PatientId);
                
                if (string.IsNullOrEmpty(patientEmail))
                {
                    _logger.LogWarning("No email found for Patient ID: {PatientId}", @event.PatientId);
                    return;
                }

                // Create notification log entry
                var notificationLog = await notificationLogService.CreateNotificationLogAsync(new CreateNotificationLogRequest
                {
                    RecipientId = @event.PatientId.ToString(),
                    RecipientEmail = patientEmail,
                    EventType = "MedicationRefillDue",
                    NotificationType = "Email",
                    Status = "Pending",
                    EventData = JsonSerializer.Serialize(@event)
                });

                // Send medication refill reminder email
                var emailRequest = new SendEmailRequest
                {
                    To = patientEmail,
                    Subject = "Nhắc nhở tái cấp thuốc - HIV Medical Center",
                    TemplateName = "MedicationRefillReminder",
                    Parameters = new Dictionary<string, object>
                    {
                        ["PatientName"] = @event.PatientName,
                        ["MedicationName"] = @event.MedicationName,
                        ["NextRefillDue"] = @event.NextRefillDue.ToString("dd/MM/yyyy"),
                        ["RefillsRemaining"] = @event.RefillsRemaining?.ToString() ?? "Không xác định"
                    }
                };

                var result = await emailService.SendEmailAsync(emailRequest);

                // Update notification log
                await notificationLogService.UpdateNotificationLogStatusAsync(notificationLog.Id, 
                    result.Success ? "Sent" : "Failed", 
                    result.Success ? null : result.Message);

                _logger.LogInformation("Medication refill reminder email sent to {Email}: {Success}", 
                    patientEmail, result.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MedicationRefillDueEvent for Patient: {PatientName}", @event.PatientName);
            }
        }

        private async Task<string> GetPatientEmail(int patientId)
        {
           
            await Task.CompletedTask;
            return "patient@example.com";
        }
    }
}
