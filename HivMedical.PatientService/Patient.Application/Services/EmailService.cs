using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Patient.Application.DTOs;
using Patient.Application.Services.Interfaces;

namespace Patient.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly INotificationTemplateService _templateService;

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger,
            INotificationTemplateService templateService)
        {
            _configuration = configuration;
            _logger = logger;
            _templateService = templateService;
        }

        public async Task<NotificationApiResponse> SendEmailAsync(SendEmailRequest request)
        {
            try
            {
                _logger.LogInformation("Sending email to {Email} with template {TemplateName}", 
                    request.To, request.TemplateName);

                // Get email template
                var template = await _templateService.GetTemplateByNameAsync(request.TemplateName);
                if (template == null)
                {
                    _logger.LogWarning("Email template {TemplateName} not found", request.TemplateName);
                    return new NotificationApiResponse 
                    { 
                        Success = false, 
                        Message = $"Email template '{request.TemplateName}' not found" 
                    };
                }

                // Process template with parameters
                var subject = ProcessTemplate(template.Subject, request.Parameters);
                var body = ProcessTemplate(template.Body, request.Parameters);

                // Send email
                using var client = CreateSmtpClient();
                using var message = CreateMailMessage(request.To, subject, body);
                
                await client.SendMailAsync(message);

                _logger.LogInformation("Email sent successfully to {Email}", request.To);
                return new NotificationApiResponse { Success = true, Message = "Email sent successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", request.To);
                return new NotificationApiResponse 
                { 
                    Success = false, 
                    Message = $"Failed to send email: {ex.Message}" 
                };
            }
        }

        public async Task<NotificationApiResponse> SendBulkEmailAsync(List<SendEmailRequest> requests)
        {
            var results = new List<string>();
            var successCount = 0;

            foreach (var request in requests)
            {
                var result = await SendEmailAsync(request);
                if (result.Success)
                {
                    successCount++;
                    results.Add($"✓ {request.To}: Success");
                }
                else
                {
                    results.Add($"✗ {request.To}: {result.Message}");
                }
            }

            return new NotificationApiResponse
            {
                Success = successCount > 0,
                Message = $"Sent {successCount}/{requests.Count} emails successfully",
                Data = results
            };
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
            var username = _configuration["Email:Username"] ?? "";
            var password = _configuration["Email:Password"] ?? "";

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            return client;
        }

        private MailMessage CreateMailMessage(string to, string subject, string body)
        {
            var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@hivmedical.com";
            var fromName = _configuration["Email:FromName"] ?? "HIV Medical Center";

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);
            return message;
        }

        private string ProcessTemplate(string template, Dictionary<string, object> parameters)
        {
            var result = template;
            
            foreach (var param in parameters)
            {
                var placeholder = $"{{{param.Key}}}";
                var value = param.Value?.ToString() ?? "";
                result = result.Replace(placeholder, value);
            }

            return result;
        }
    }
}
