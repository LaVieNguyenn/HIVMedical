using Microsoft.Extensions.Logging;
using Patient.Application.Services.Interfaces;
using Patient.Domain.Entities;
using Patient.Application.DTOs;

namespace Patient.Application.Services
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly ILogger<NotificationTemplateService> _logger;
        
        // For demo purposes, using in-memory templates
        private readonly Dictionary<string, NotificationTemplate> _templates;

        public NotificationTemplateService(ILogger<NotificationTemplateService> logger)
        {
            _logger = logger;
            _templates = InitializeDefaultTemplates();
        }

        public async Task<NotificationTemplateDto?> GetTemplateByNameAsync(string templateName)
        {
            await Task.CompletedTask;
            
            if (_templates.TryGetValue(templateName, out var template))
            {
                return MapToDto(template);
            }

            return null;
        }

        public async Task<NotificationApiResponse<NotificationTemplateDto>> CreateTemplateAsync(CreateNotificationTemplateRequest request)
        {
            await Task.CompletedTask;
            
            try
            {
                if (_templates.ContainsKey(request.TemplateName))
                {
                    return new NotificationApiResponse<NotificationTemplateDto>
                    {
                        Success = false,
                        Message = "Template with this name already exists"
                    };
                }

                var template = new NotificationTemplate
                {
                    Id = _templates.Count + 1,
                    TemplateName = request.TemplateName,
                    Subject = request.Subject,
                    Body = request.Body,
                    NotificationType = request.NotificationType,
                    Language = request.Language,
                    Parameters = request.Parameters,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _templates[request.TemplateName] = template;

                return new NotificationApiResponse<NotificationTemplateDto>
                {
                    Success = true,
                    Message = "Template created successfully",
                    Data = MapToDto(template)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification template");
                return new NotificationApiResponse<NotificationTemplateDto>
                {
                    Success = false,
                    Message = "Failed to create template"
                };
            }
        }

        public async Task<NotificationApiResponse<NotificationTemplateDto>> UpdateTemplateAsync(int id, UpdateNotificationTemplateRequest request)
        {
            await Task.CompletedTask;
            
            var template = _templates.Values.FirstOrDefault(t => t.Id == id);
            if (template == null)
            {
                return new NotificationApiResponse<NotificationTemplateDto>
                {
                    Success = false,
                    Message = "Template not found"
                };
            }

            template.Subject = request.Subject;
            template.Body = request.Body;
            template.IsActive = request.IsActive;
            template.Parameters = request.Parameters;
            template.UpdatedAt = DateTime.UtcNow;

            return new NotificationApiResponse<NotificationTemplateDto>
            {
                Success = true,
                Message = "Template updated successfully",
                Data = MapToDto(template)
            };
        }

        public async Task<NotificationApiResponse> DeleteTemplateAsync(int id)
        {
            await Task.CompletedTask;
            
            var template = _templates.Values.FirstOrDefault(t => t.Id == id);
            if (template == null)
            {
                return new NotificationApiResponse
                {
                    Success = false,
                    Message = "Template not found"
                };
            }

            _templates.Remove(template.TemplateName);
            
            return new NotificationApiResponse
            {
                Success = true,
                Message = "Template deleted successfully"
            };
        }

        public async Task<NotificationApiResponse<IEnumerable<NotificationTemplateDto>>> GetAllTemplatesAsync()
        {
            await Task.CompletedTask;
            
            var templates = _templates.Values.Select(MapToDto);
            
            return new NotificationApiResponse<IEnumerable<NotificationTemplateDto>>
            {
                Success = true,
                Data = templates
            };
        }

        private NotificationTemplateDto MapToDto(NotificationTemplate template)
        {
            return new NotificationTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                Subject = template.Subject,
                Body = template.Body,
                NotificationType = template.NotificationType,
                IsActive = template.IsActive,
                Language = template.Language,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt
            };
        }

        private Dictionary<string, NotificationTemplate> InitializeDefaultTemplates()
        {
            return new Dictionary<string, NotificationTemplate>
            {
                ["AppointmentConfirmation"] = new NotificationTemplate
                {
                    Id = 1,
                    TemplateName = "AppointmentConfirmation",
                    Subject = "Xác nhận lịch hẹn khám - HIV Medical Center",
                    Body = @"
                        <html>
                        <body>
                            <h2>Xác nhận lịch hẹn khám</h2>
                            <p>Kính chào {PatientName},</p>
                            <p>Chúng tôi xác nhận lịch hẹn khám của bạn như sau:</p>
                            <ul>
                                <li><strong>Ngày khám:</strong> {AppointmentDate}</li>
                                <li><strong>Giờ khám:</strong> {AppointmentTime}</li>
                                <li><strong>Loại khám:</strong> {AppointmentType}</li>
                                <li><strong>Lý do khám:</strong> {Reason}</li>
                            </ul>
                            <p><strong>Ghi chú:</strong> {Notes}</p>
                            <p>Vui lòng đến đúng giờ và mang theo các giấy tờ cần thiết.</p>
                            <p>Trân trọng,<br/>HIV Medical Center</p>
                        </body>
                        </html>",
                    NotificationType = "Email",
                    IsActive = true,
                    Language = "vi-VN",
                    CreatedAt = DateTime.UtcNow
                },
                
                ["AppointmentCancellation"] = new NotificationTemplate
                {
                    Id = 2,
                    TemplateName = "AppointmentCancellation",
                    Subject = "Thông báo hủy lịch hẹn - HIV Medical Center",
                    Body = @"
                        <html>
                        <body>
                            <h2>Thông báo hủy lịch hẹn</h2>
                            <p>Kính chào {PatientName},</p>
                            <p>Chúng tôi thông báo lịch hẹn của bạn đã được hủy:</p>
                            <ul>
                                <li><strong>Ngày khám:</strong> {AppointmentDate}</li>
                                <li><strong>Giờ khám:</strong> {AppointmentTime}</li>
                                <li><strong>Loại khám:</strong> {AppointmentType}</li>
                                <li><strong>Lý do hủy:</strong> {CancellationReason}</li>
                            </ul>
                            <p>Nếu bạn cần đặt lịch hẹn mới, vui lòng liên hệ với chúng tôi.</p>
                            <p>Trân trọng,<br/>HIV Medical Center</p>
                        </body>
                        </html>",
                    NotificationType = "Email",
                    IsActive = true,
                    Language = "vi-VN",
                    CreatedAt = DateTime.UtcNow
                },
                
                ["MedicationPrescribed"] = new NotificationTemplate
                {
                    Id = 3,
                    TemplateName = "MedicationPrescribed",
                    Subject = "Thông tin đơn thuốc mới - HIV Medical Center",
                    Body = @"
                        <html>
                        <body>
                            <h2>Thông tin đơn thuốc mới</h2>
                            <p>Kính chào {PatientName},</p>
                            <p>Bác sĩ đã kê đơn thuốc mới cho bạn:</p>
                            <ul>
                                <li><strong>Tên thuốc:</strong> {MedicationName}</li>
                                <li><strong>Liều dùng:</strong> {Dosage}</li>
                                <li><strong>Tần suất:</strong> {Frequency}</li>
                                <li><strong>Hướng dẫn sử dụng:</strong> {Instructions}</li>
                                <li><strong>Ngày bắt đầu:</strong> {StartDate}</li>
                            </ul>
                            <p><strong>Ghi chú từ bác sĩ:</strong> {Notes}</p>
                            <p>Vui lòng tuân thủ đúng liều lượng và tần suất như hướng dẫn.</p>
                            <p>Trân trọng,<br/>HIV Medical Center</p>
                        </body>
                        </html>",
                    NotificationType = "Email",
                    IsActive = true,
                    Language = "vi-VN",
                    CreatedAt = DateTime.UtcNow
                },
                
                ["MedicationRefillReminder"] = new NotificationTemplate
                {
                    Id = 4,
                    TemplateName = "MedicationRefillReminder",
                    Subject = "Nhắc nhở tái cấp thuốc - HIV Medical Center",
                    Body = @"
                        <html>
                        <body>
                            <h2>Nhắc nhở tái cấp thuốc</h2>
                            <p>Kính chào {PatientName},</p>
                            <p>Đây là thông báo nhắc nhở về việc tái cấp thuốc:</p>
                            <ul>
                                <li><strong>Tên thuốc:</strong> {MedicationName}</li>
                                <li><strong>Ngày hết hạn:</strong> {NextRefillDue}</li>
                                <li><strong>Số lần tái cấp còn lại:</strong> {RefillsRemaining}</li>
                            </ul>
                            <p>Vui lòng liên hệ với chúng tôi để được tái cấp thuốc kịp thời.</p>
                            <p>Trân trọng,<br/>HIV Medical Center</p>
                        </body>
                        </html>",
                    NotificationType = "Email",
                    IsActive = true,
                    Language = "vi-VN",
                    CreatedAt = DateTime.UtcNow
                },

                ["PatientCreated"] = new NotificationTemplate
                {
                    Id = 5,
                    TemplateName = "PatientCreated",
                    Subject = "Chào mừng bạn đến với HIV Medical Center",
                    Body = @"
                        <html>
                        <body>
                            <h2>Chào mừng bạn đến với HIV Medical Center</h2>
                            <p>Kính chào {PatientName},</p>
                            <p>Chúng tôi xin chào mừng bạn đến với HIV Medical Center. Hồ sơ bệnh nhân của bạn đã được tạo thành công:</p>
                            <ul>
                                <li><strong>Mã bệnh nhân:</strong> {PatientCode}</li>
                                <li><strong>Họ tên:</strong> {PatientName}</li>
                                <li><strong>Email:</strong> {Email}</li>
                                <li><strong>Ngày tạo hồ sơ:</strong> {CreatedDate}</li>
                            </ul>
                            <p>Bạn có thể sử dụng mã bệnh nhân để đặt lịch hẹn và theo dõi quá trình điều trị.</p>
                            <p>Nếu có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi.</p>
                            <p>Trân trọng,<br/>HIV Medical Center</p>
                        </body>
                        </html>",
                    NotificationType = "Email",
                    IsActive = true,
                    Language = "vi-VN",
                    CreatedAt = DateTime.UtcNow
                }
            };
        }
    }
}
