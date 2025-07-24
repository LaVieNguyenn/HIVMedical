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
    public class PatientController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientController(PatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var result = await _patientService.GetPatientByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authUserId))
            {
                return Unauthorized("Invalid token");
            }

            var result = await _patientService.GetPatientByAuthUserIdAsync(authUserId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        [Authorize]
        //[Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can see all patients
        public async Task<IActionResult> GetAllPatients()
        {
            var result = await _patientService.GetAllPatientsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authUserId))
            {
                return Unauthorized("Invalid token");
            }

            // Set the AuthUserId from the token
            request.AuthUserId = authUserId;

            var result = await _patientService.CreatePatientAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetPatient), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientRequest request)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authUserId))
            {
                return Unauthorized("Invalid token");
            }

            // Check if user is updating their own profile or if they're a doctor/admin
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Doctor" && userRole != "Admin")
            {
                // Regular users can only update their own profile
                var patientResult = await _patientService.GetPatientByIdAsync(id);
                if (!patientResult.Success || patientResult.Data?.AuthUserId != authUserId)
                {
                    return Forbid("You can only update your own profile");
                }
            }

            var result = await _patientService.UpdatePatientAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

