using Authentication.Application.DTOs;
using Authentication.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")] // Only Admin role can access these endpoints
    public class AdminController : ControllerBase
    {
        private readonly UserManagementService _userManagementService;

        public AdminController(UserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

       
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilterRequest filter)
        {
            var result = await _userManagementService.GetUsersAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

       
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _userManagementService.GetUserByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userManagementService.CreateUserAsync(request);
            return result.Success ? 
                CreatedAtAction(nameof(GetUser), new { id = result.Data?.Id }, result) : 
                BadRequest(result);
        }

        
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userManagementService.UpdateUserAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

       
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userManagementService.DeleteUserAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

     
        [HttpPost("users/{id}/restore")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            var result = await _userManagementService.RestoreUserAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

       
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _userManagementService.GetRolesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
