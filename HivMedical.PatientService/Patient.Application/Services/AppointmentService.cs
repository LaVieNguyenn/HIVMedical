using Patient.Application.DTOs;
using Patient.Application.Events;
using Patient.Domain.Entities;
using Patient.Infrastructure.UnitOfWorks;
using SharedLibrary.Messaging;
using SharedLibrary.Response;

namespace Patient.Application.Services
{
    public class AppointmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEventBus _eventBus;

        public AppointmentService(IUnitOfWork unitOfWork, IEventBus eventBus)
        {
            _uow = unitOfWork;
            _eventBus = eventBus;
        }

        public async Task<ApiResponse<AppointmentDto>> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(id);
            if (appointment == null)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = "Appointment not found"
                };
            }

            var appointmentDto = MapToDto(appointment);
            return new ApiResponse<AppointmentDto>
            {
                Success = true,
                Data = appointmentDto
            };
        }

        public async Task<ApiResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            var appointments = await _uow.Appointments.GetByPatientIdAsync(patientId);
            var appointmentDtos = appointments.Select(MapToDto);

            return new ApiResponse<IEnumerable<AppointmentDto>>
            {
                Success = true,
                Data = appointmentDtos
            };
        }

        public async Task<ApiResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            var appointments = await _uow.Appointments.GetByDoctorIdAsync(doctorId);
            var appointmentDtos = appointments.Select(MapToDto);

            return new ApiResponse<IEnumerable<AppointmentDto>>
            {
                Success = true,
                Data = appointmentDtos
            };
        }

        public async Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentRequest request)
        {
            // Verify patient exists
            var patient = await _uow.Patients.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = "Patient not found"
                };
            }

            // Check for conflicting appointments
            var hasConflict = await _uow.Appointments.HasConflictingAppointmentAsync(
                request.DoctorId, request.AppointmentDate, request.AppointmentTime);
            
            if (hasConflict)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = "Doctor already has an appointment at this time"
                };
            }

            // Validate appointment date is not in the past
            var appointmentDateTime = request.AppointmentDate.Date.Add(request.AppointmentTime);
            if (appointmentDateTime <= DateTime.Now)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = "Cannot schedule appointment in the past"
                };
            }

            var appointment = new Appointment
            {
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                AppointmentDate = request.AppointmentDate.Date,
                AppointmentTime = request.AppointmentTime,
                AppointmentType = request.AppointmentType,
                Status = "Scheduled",
                Reason = request.Reason,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _uow.Appointments.CreateAsync(appointment);
            if (!result.Success)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            // Get the appointment with patient info
            var createdAppointment = await _uow.Appointments.GetByIdAsync(appointment.Id);

            // Publish AppointmentCreatedEvent
            try
            {
                var appointmentCreatedEvent = new AppointmentCreatedEvent(
                    appointment.Id,
                    appointment.PatientId,
                    createdAppointment?.Patient?.FullName ?? "",
                    createdAppointment?.Patient?.PatientCode ?? "",
                    appointment.DoctorId,
                    appointment.AppointmentDate,
                    appointment.AppointmentTime,
                    appointment.AppointmentType,
                    appointment.Reason,
                    appointment.Notes,
                    appointment.CreatedAt
                );

                _eventBus.Publish(appointmentCreatedEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish AppointmentCreatedEvent: {ex.Message}");
            }

            var appointmentDto = MapToDto(createdAppointment!);

            return new ApiResponse<AppointmentDto>
            {
                Success = true,
                Message = "Appointment created successfully",
                Data = appointmentDto
            };
        }

        public async Task<ApiResponse<AppointmentDto>> UpdateAppointmentAsync(int id, UpdateAppointmentRequest request)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(id);
            if (appointment == null)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = "Appointment not found"
                };
            }

            // Check if appointment can be updated (not completed or cancelled)
            if (appointment.Status == "Completed" || appointment.Status == "Cancelled")
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = "Cannot update completed or cancelled appointment"
                };
            }

            // Check for conflicting appointments if date/time changed
            if (appointment.AppointmentDate != request.AppointmentDate.Date || 
                appointment.AppointmentTime != request.AppointmentTime)
            {
                var hasConflict = await _uow.Appointments.HasConflictingAppointmentAsync(
                    appointment.DoctorId, request.AppointmentDate, request.AppointmentTime, id);
                
                if (hasConflict)
                {
                    return new ApiResponse<AppointmentDto>
                    {
                        Success = false,
                        Message = "Doctor already has an appointment at this time"
                    };
                }
            }

            // Validate appointment date is not in the past
            var appointmentDateTime = request.AppointmentDate.Date.Add(request.AppointmentTime);
            if (appointmentDateTime <= DateTime.Now)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = "Cannot schedule appointment in the past"
                };
            }

            // Update appointment properties
            appointment.AppointmentDate = request.AppointmentDate.Date;
            appointment.AppointmentTime = request.AppointmentTime;
            appointment.AppointmentType = request.AppointmentType;
            appointment.Reason = request.Reason;
            appointment.Notes = request.Notes;
            appointment.UpdatedAt = DateTime.UtcNow;

            var result = await _uow.Appointments.UpdateAsync(appointment);
            if (!result.Success)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var appointmentDto = MapToDto(appointment);
            return new ApiResponse<AppointmentDto>
            {
                Success = true,
                Message = "Appointment updated successfully",
                Data = appointmentDto
            };
        }

        public async Task<ApiResponse<AppointmentDto>> UpdateAppointmentStatusAsync(int id, UpdateAppointmentStatusRequest request)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(id);
            if (appointment == null)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = "Appointment not found"
                };
            }

            // Validate status
            var validStatuses = new[] { "Scheduled", "Completed", "Cancelled", "No-Show" };
            if (!validStatuses.Contains(request.Status))
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = "Invalid appointment status"
                };
            }

            appointment.Status = request.Status;
            if (!string.IsNullOrEmpty(request.Notes))
            {
                appointment.Notes = request.Notes;
            }
            appointment.UpdatedAt = DateTime.UtcNow;

            var result = await _uow.Appointments.UpdateAsync(appointment);
            if (!result.Success)
            {
                return new ApiResponse<AppointmentDto>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            var appointmentDto = MapToDto(appointment);
            return new ApiResponse<AppointmentDto>
            {
                Success = true,
                Message = $"Appointment status updated to {request.Status}",
                Data = appointmentDto
            };
        }

        public async Task<ApiResponse<bool>> DeleteAppointmentAsync(int id)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(id);
            if (appointment == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Appointment not found"
                };
            }

            // Only allow deletion of scheduled appointments
            if (appointment.Status != "Scheduled")
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Can only delete scheduled appointments"
                };
            }

            var result = await _uow.Appointments.DeleteAsync(appointment);
            if (!result.Success)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            await _uow.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Appointment deleted successfully",
                Data = true
            };
        }

        public async Task<ApiResponse<IEnumerable<AppointmentDto>>> GetFilteredAppointmentsAsync(AppointmentFilterRequest filter)
        {
            var appointments = await _uow.Appointments.GetFilteredAppointmentsAsync(
                filter.PatientId,
                filter.DoctorId,
                filter.FromDate,
                filter.ToDate,
                filter.Status,
                filter.AppointmentType,
                filter.PageNumber,
                filter.PageSize);
            var appointmentDtos = appointments.Select(MapToDto);

            return new ApiResponse<IEnumerable<AppointmentDto>>
            {
                Success = true,
                Data = appointmentDtos
            };
        }

        public async Task<ApiResponse<AppointmentSummaryDto>> GetAppointmentSummaryByPatientIdAsync(int patientId)
        {
            var allAppointments = await _uow.Appointments.GetByPatientIdAsync(patientId);
            var nextAppointment = await _uow.Appointments.GetNextAppointmentByPatientIdAsync(patientId);

            var summary = new AppointmentSummaryDto
            {
                TotalAppointments = allAppointments.Count(),
                ScheduledAppointments = allAppointments.Count(a => a.Status == "Scheduled"),
                CompletedAppointments = allAppointments.Count(a => a.Status == "Completed"),
                CancelledAppointments = allAppointments.Count(a => a.Status == "Cancelled"),
                NoShowAppointments = allAppointments.Count(a => a.Status == "No-Show"),
                NextAppointment = nextAppointment != null ? MapToDto(nextAppointment) : null
            };

            return new ApiResponse<AppointmentSummaryDto>
            {
                Success = true,
                Data = summary
            };
        }

        public async Task<ApiResponse<IEnumerable<AppointmentDto>>> GetUpcomingAppointmentsByPatientIdAsync(int patientId, int days = 7)
        {
            var appointments = await _uow.Appointments.GetUpcomingAppointmentsByPatientIdAsync(patientId, days);
            var appointmentDtos = appointments.Select(MapToDto);

            return new ApiResponse<IEnumerable<AppointmentDto>>
            {
                Success = true,
                Data = appointmentDtos
            };
        }

        public async Task<ApiResponse<IEnumerable<AppointmentDto>>> GetTodayAppointmentsByDoctorAsync(int doctorId)
        {
            var appointments = await _uow.Appointments.GetTodayAppointmentsByDoctorAsync(doctorId);
            var appointmentDtos = appointments.Select(MapToDto);

            return new ApiResponse<IEnumerable<AppointmentDto>>
            {
                Success = true,
                Data = appointmentDtos
            };
        }

        private static AppointmentDto MapToDto(Appointment appointment)
        {
            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                AppointmentType = appointment.AppointmentType,
                Status = appointment.Status,
                Notes = appointment.Notes,
                Reason = appointment.Reason,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt,
                PatientName = appointment.Patient?.FullName,
                PatientCode = appointment.Patient?.PatientCode
            };
        }
    }
}
