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
    public class MedicationController : ControllerBase
    {
        private readonly MedicationService _medicationService;

        public MedicationController(MedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedication(int id)
        {
            var result = await _medicationService.GetMedicationByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMedications()
        {
            var result = await _medicationService.GetAllMedicationsAsync();
            return Ok(result);
        }

        [HttpGet("hiv")]
        public async Task<IActionResult> GetHIVMedications()
        {
            var result = await _medicationService.GetHIVMedicationsAsync();
            return Ok(result);
        }

        [HttpGet("arv")]
        public async Task<IActionResult> GetARVMedications()
        {
            var result = await _medicationService.GetARVMedicationsAsync();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMedications([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            var result = await _medicationService.SearchMedicationsAsync(searchTerm);
            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredMedications([FromQuery] MedicationFilterRequest filter)
        {
            var result = await _medicationService.GetFilteredMedicationsAsync(filter);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can create medications
        public async Task<IActionResult> CreateMedication([FromBody] CreateMedicationRequest request)
        {
            var result = await _medicationService.CreateMedicationAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetMedication), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")] // Only doctors and admins can update medications
        public async Task<IActionResult> UpdateMedication(int id, [FromBody] UpdateMedicationRequest request)
        {
            var result = await _medicationService.UpdateMedicationAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only admins can delete medications
        public async Task<IActionResult> DeleteMedication(int id)
        {
            var result = await _medicationService.DeleteMedicationAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
