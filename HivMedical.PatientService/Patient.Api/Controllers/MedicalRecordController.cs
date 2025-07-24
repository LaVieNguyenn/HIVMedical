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
    public class MedicalRecordController : ControllerBase
    {
        private readonly MedicalRecordService _medicalRecordService;
        private readonly PatientService _patientService;

        public MedicalRecordController(MedicalRecordService medicalRecordService, PatientService patientService)
        {
            _medicalRecordService = medicalRecordService;
            _patientService = patientService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalRecord(int id)
        {
            var result = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
            if (!result.Success)
                return NotFound(result);

            // Check if user has permission to view this record
            var hasPermission = await CheckRecordPermission(result.Data!.PatientId);
            if (!hasPermission)
                return Forbid("You don't have permission to view this record");

            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetMedicalRecordsByPatient(int patientId)
        {
            // Check if user has permission to view records for this patient
            var hasPermission = await CheckRecordPermission(patientId);
            if (!hasPermission)
                return Forbid("You don't have permission to view these records");

            var result = await _medicalRecordService.GetMedicalRecordsByPatientIdAsync(patientId);
            return Ok(result);
        }

        [HttpGet("my-records")]
        public async Task<IActionResult> GetMyMedicalRecords()
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

            var result = await _medicalRecordService.GetMedicalRecordsByPatientIdAsync(patientResult.Data!.Id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can create medical records
        public async Task<IActionResult> CreateMedicalRecord([FromBody] CreateMedicalRecordRequest request)
        {
            // Get current user ID from JWT token to set as DoctorId
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int doctorId))
            {
                return Unauthorized("Invalid token");
            }

            request.DoctorId = doctorId;

            var result = await _medicalRecordService.CreateMedicalRecordAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetMedicalRecord), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can update medical records
        public async Task<IActionResult> UpdateMedicalRecord(int id, [FromBody] UpdateMedicalRecordRequest request)
        {
            var result = await _medicalRecordService.UpdateMedicalRecordAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can delete medical records
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var result = await _medicalRecordService.DeleteMedicalRecordAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private async Task<bool> CheckRecordPermission(int patientId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Doctors and Admins can view all records
            if (userRole == "Doctor" || userRole == "Admin")
                return true;

            // Regular users can only view their own records
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int authUserId))
            {
                var patientResult = await _patientService.GetPatientByIdAsync(patientId);
                return patientResult.Success && patientResult.Data?.AuthUserId == authUserId;
            }

            return false;
        }
    }
}

