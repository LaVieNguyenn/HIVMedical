using Doctor.Application.DTOs;
using Doctor.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Doctor.Api.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorService _service;

        public DoctorController(DoctorService service)
        {
            _service = service;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var doctor = await _service.GetByIdAsync(id);
            return doctor == null ? NotFound() : Ok(doctor);
        }
        [HttpPost]
        [Authorize(Roles = "Staff,Manager,Admin")]
        public async Task<IActionResult> Create([FromBody] DoctorCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.DoctorId }, created);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Staff,Manager,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] DoctorUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole == "Doctor")
            {
                var doctor = await _service.GetByIdAsync(id);
                if (doctor == null)
                    return NotFound();

                if (doctor.UserId.ToString() != userId)
                    return Forbid("Bạn chỉ được phép sửa thông tin cá nhân của mình!");
            }

            var result = await _service.UpdateAsync(id, dto);
            return result ? Ok() : NotFound();
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
