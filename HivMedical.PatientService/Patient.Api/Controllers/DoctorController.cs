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
    public class DoctorController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var result = await _doctorService.GetAllDoctorsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(int id)
        {
            var result = await _doctorService.GetDoctorByIdAsync(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyProfile()
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authUserId))
            {
                return Unauthorized("Invalid token");
            }

            var result = await _doctorService.GetDoctorByAuthUserIdAsync(authUserId);
            if (!result.Success)
                return NotFound("Doctor profile not found");

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Only admins can create doctor profiles
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorRequest request)
        {
            var result = await _doctorService.CreateDoctorAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetDoctor), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorRequest request)
        {
            // Get current user info
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authUserId))
            {
                return Unauthorized("Invalid token");
            }

            // Check permissions based on role
            if (userRole != "Admin")
            {
                // Regular doctors can only update their own profile
                var doctorResult = await _doctorService.GetDoctorByIdAsync(id);
                if (!doctorResult.Success)
                    return NotFound("Doctor not found");
                
                if (doctorResult.Data!.Id != id)
                    return Forbid("You can only update your own profile");
            }

            var result = await _doctorService.UpdateDoctorAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can delete doctor profiles
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

