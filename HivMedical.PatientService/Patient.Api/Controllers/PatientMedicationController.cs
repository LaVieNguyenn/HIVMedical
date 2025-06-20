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
    public class PatientMedicationController : ControllerBase
    {
        private readonly PatientMedicationService _patientMedicationService;
        private readonly PatientService _patientService;

        public PatientMedicationController(PatientMedicationService patientMedicationService, PatientService patientService)
        {
            _patientMedicationService = patientMedicationService;
            _patientService = patientService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientMedication(int id)
        {
            var result = await _patientMedicationService.GetPatientMedicationByIdAsync(id);
            if (!result.Success)
                return NotFound(result);

            // Check if user has permission to view this medication
            var hasPermission = await CheckPatientMedicationPermission(result.Data!.PatientId);
            if (!hasPermission)
                return Forbid("You don't have permission to view this medication");

            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientMedications(int patientId)
        {
            // Check if user has permission to view medications for this patient
            var hasPermission = await CheckPatientPermission(patientId);
            if (!hasPermission)
                return Forbid("You don't have permission to view these medications");

            var result = await _patientMedicationService.GetPatientMedicationsByPatientIdAsync(patientId);
            return Ok(result);
        }

        [HttpGet("patient/{patientId}/current")]
        public async Task<IActionResult> GetCurrentPatientMedications(int patientId)
        {
            // Check if user has permission to view medications for this patient
            var hasPermission = await CheckPatientPermission(patientId);
            if (!hasPermission)
                return Forbid("You don't have permission to view these medications");

            var result = await _patientMedicationService.GetCurrentMedicationsByPatientIdAsync(patientId);
            return Ok(result);
        }

        [HttpGet("my-medications")]
        public async Task<IActionResult> GetMyMedications()
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

            var result = await _patientMedicationService.GetPatientMedicationsByPatientIdAsync(patientResult.Data!.Id);
            return Ok(result);
        }

        [HttpGet("my-current-medications")]
        public async Task<IActionResult> GetMyCurrentMedications()
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

            var result = await _patientMedicationService.GetCurrentMedicationsByPatientIdAsync(patientResult.Data!.Id);
            return Ok(result);
        }

        [HttpGet("my-summary")]
        public async Task<IActionResult> GetMyMedicationSummary()
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

            var result = await _patientMedicationService.GetPatientMedicationSummaryAsync(patientResult.Data!.Id);
            return Ok(result);
        }

        [HttpGet("patient/{patientId}/summary")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can view patient summaries
        public async Task<IActionResult> GetPatientMedicationSummary(int patientId)
        {
            var result = await _patientMedicationService.GetPatientMedicationSummaryAsync(patientId);
            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredPatientMedications([FromQuery] PatientMedicationFilterRequest filter)
        {
            // Apply permission filters based on user role
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int authUserId))
            {
                if (userRole == "Doctor")
                {
                    // Doctors can only see medications they prescribed
                    filter.PrescribedByDoctorId = authUserId;
                }
                else if (userRole != "Admin")
                {
                    // Regular users can only see their own medications
                    var patientResult = await _patientService.GetPatientByAuthUserIdAsync(authUserId);
                    if (!patientResult.Success)
                        return NotFound("Patient profile not found");
                    
                    filter.PatientId = patientResult.Data!.Id;
                }
                // Admins can see all medications without restrictions
            }

            var result = await _patientMedicationService.GetFilteredPatientMedicationsAsync(filter);
            return Ok(result);
        }

        [HttpPost("prescribe")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can prescribe medications
        public async Task<IActionResult> PrescribeMedication([FromBody] CreatePatientMedicationRequest request)
        {
            // Get current user ID from JWT token to set as prescribing doctor
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int doctorId))
            {
                return Unauthorized("Invalid token");
            }

            request.PrescribedByDoctorId = doctorId;

            var result = await _patientMedicationService.PrescribeMedicationAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetPatientMedication), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can update medications
        public async Task<IActionResult> UpdatePatientMedication(int id, [FromBody] UpdatePatientMedicationRequest request)
        {
            var result = await _patientMedicationService.UpdatePatientMedicationAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id}/discontinue")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can discontinue medications
        public async Task<IActionResult> DiscontinueMedication(int id, [FromBody] string reason)
        {
            var result = await _patientMedicationService.DiscontinueMedicationAsync(id, reason);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private async Task<bool> CheckPatientMedicationPermission(int patientId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Doctors and Admins can view all patient medications
            if (userRole == "Doctor" || userRole == "Admin")
                return true;

            // Regular users can only view their own medications
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int authUserId))
            {
                var patientResult = await _patientService.GetPatientByIdAsync(patientId);
                return patientResult.Success && patientResult.Data?.AuthUserId == authUserId;
            }

            return false;
        }

        private async Task<bool> CheckPatientPermission(int patientId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Doctors and Admins can view all patient data
            if (userRole == "Doctor" || userRole == "Admin")
                return true;

            // Regular users can only view their own data
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int authUserId))
            {
                var patientResult = await _patientService.GetPatientByIdAsync(patientId);
                return patientResult.Success && patientResult.Data?.AuthUserId == authUserId;
            }

            return false;
        }
    }
}
