using Doctor.Application.DTOs;
using Doctor.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.Api.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController : Controller
    {
        private readonly DoctorService _service;
        public DoctorController(DoctorService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var doctor = await _service.GetByIdAsync(id);
            return doctor == null ? NotFound() : Ok(doctor);
        }
    }
}
