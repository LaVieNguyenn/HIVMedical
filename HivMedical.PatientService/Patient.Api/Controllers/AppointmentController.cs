using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Patient.Application.DTOs;
using Patient.Application.Services;
using System.Security.Claims;

namespace Patient.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require JWT authentication for all endpoints
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        private readonly PatientService _patientService;

        public AppointmentController(AppointmentService appointmentService, PatientService patientService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(int id)
        {
            var result = await _appointmentService.GetAppointmentByIdAsync(id);
            if (!result.Success)
                return NotFound(result);

            // Check if user has permission to view this appointment
            var hasPermission = await CheckAppointmentPermission(result.Data!.PatientId, result.Data.DoctorId);
            if (!hasPermission)
                return Forbid("You don't have permission to view this appointment");

            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatient(int patientId)
        {
            // Check if user has permission to view appointments for this patient
            var hasPermission = await CheckPatientPermission(patientId);
            if (!hasPermission)
                return Forbid("You don't have permission to view these appointments");

            var result = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            return Ok(result);
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can view doctor's appointments
        public async Task<IActionResult> GetAppointmentsByDoctor(int doctorId)
        {
            // Additional check: doctors can only view their own appointments unless they're admin
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userRole == "Doctor" && userIdClaim != null && int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                if (currentUserId != doctorId)
                    return Forbid("You can only view your own appointments");
            }

            var result = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authUserId))
            {
                return Unauthorized("Invalid token");
            }

            if (userRole == "Doctor")
            {
                // For doctors, return their appointments as doctor
                var result = await _appointmentService.GetAppointmentsByDoctorIdAsync(authUserId);
                return Ok(result);
            }
            else
            {
                // For patients, get patient profile first
                var patientResult = await _patientService.GetPatientByAuthUserIdAsync(authUserId);
                if (!patientResult.Success)
                    return NotFound("Patient profile not found");

                var result = await _appointmentService.GetAppointmentsByPatientIdAsync(patientResult.Data!.Id);
                return Ok(result);
            }
        }

        [HttpGet("my-summary")]
        public async Task<IActionResult> GetMyAppointmentSummary()
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authUserId))
            {
                return Unauthorized("Invalid token");
            }

            // Get patient by auth user ID
            var patientResult = await _patientService.GetPatientByAuthUserIdAsync(authUserId);
            if (!patientResult.Success)
                return NotFound("Patient profile not found");

            var result = await _appointmentService.GetAppointmentSummaryByPatientIdAsync(patientResult.Data!.Id);
            return Ok(result);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingAppointments([FromQuery] int days = 7)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authUserId))
            {
                return Unauthorized("Invalid token");
            }

            // Get patient by auth user ID
            var patientResult = await _patientService.GetPatientByAuthUserIdAsync(authUserId);
            if (!patientResult.Success)
                return NotFound("Patient profile not found");

            var result = await _appointmentService.GetUpcomingAppointmentsByPatientIdAsync(patientResult.Data!.Id, days);
            return Ok(result);
        }

        [HttpGet("today")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can view today's appointments
        public async Task<IActionResult> GetTodayAppointments()
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int doctorId))
            {
                return Unauthorized("Invalid token");
            }

            var result = await _appointmentService.GetTodayAppointmentsByDoctorAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredAppointments([FromQuery] AppointmentFilterRequest filter)
        {
            // Apply permission filters based on user role
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int authUserId))
            {
                if (userRole == "Doctor")
                {
                    // Doctors can only see their own appointments
                    filter.DoctorId = authUserId;
                }
                else if (userRole != "Admin")
                {
                    // Regular users can only see their own appointments as patients
                    var patientResult = await _patientService.GetPatientByAuthUserIdAsync(authUserId);
                    if (!patientResult.Success)
                        return NotFound("Patient profile not found");
                    
                    filter.PatientId = patientResult.Data!.Id;
                }
                // Admins can see all appointments without restrictions
            }

            var result = await _appointmentService.GetFilteredAppointmentsAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
        {
            // Get current user info
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authUserId))
            {
                return Unauthorized("Invalid token");
            }

            // Check permissions based on role
            if (userRole == "Doctor" || userRole == "Admin")
            {
                // Doctors and admins can create appointments for any patient
                // DoctorId and PatientId should be provided in the request
            }
            else
            {
                // Regular users can only create appointments for themselves
                var patientResult = await _patientService.GetPatientByAuthUserIdAsync(authUserId);
                if (!patientResult.Success)
                    return NotFound("Patient profile not found");
                
                request.PatientId = patientResult.Data!.Id;
                // DoctorId should be provided in the request
            }

            var result = await _appointmentService.CreateAppointmentAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetAppointment), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentRequest request)
        {
            // Get the appointment first to check permissions
            var appointmentResult = await _appointmentService.GetAppointmentByIdAsync(id);
            if (!appointmentResult.Success)
                return NotFound(appointmentResult);

            // Check if user has permission to update this appointment
            var hasPermission = await CheckAppointmentPermission(appointmentResult.Data!.PatientId, appointmentResult.Data.DoctorId);
            if (!hasPermission)
                return Forbid("You don't have permission to update this appointment");

            var result = await _appointmentService.UpdateAppointmentAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can update appointment status
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusRequest request)
        {
            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            // Get the appointment first to check permissions
            var appointmentResult = await _appointmentService.GetAppointmentByIdAsync(id);
            if (!appointmentResult.Success)
                return NotFound(appointmentResult);

            // Check if user has permission to delete this appointment
            var hasPermission = await CheckAppointmentPermission(appointmentResult.Data!.PatientId, appointmentResult.Data.DoctorId);
            if (!hasPermission)
                return Forbid("You don't have permission to delete this appointment");

            var result = await _appointmentService.DeleteAppointmentAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private async Task<bool> CheckAppointmentPermission(int patientId, int doctorId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Admins can access all appointments
            if (userRole == "Admin")
                return true;

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int authUserId))
            {
                // Doctors can access appointments where they are the doctor
                if (userRole == "Doctor" && authUserId == doctorId)
                    return true;

                // Patients can access their own appointments
                var patientResult = await _patientService.GetPatientByIdAsync(patientId);
                if (patientResult.Success && patientResult.Data?.AuthUserId == authUserId)
                    return true;
            }

            return false;
        }

        private async Task<bool> CheckPatientPermission(int patientId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Doctors and Admins can view all patient appointments
            if (userRole == "Doctor" || userRole == "Admin")
                return true;

            // Regular users can only view their own appointments
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int authUserId))
            {
                var patientResult = await _patientService.GetPatientByIdAsync(patientId);
                return patientResult.Success && patientResult.Data?.AuthUserId == authUserId;
            }

            return false;
        }
    }
}
