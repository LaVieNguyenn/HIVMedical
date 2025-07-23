using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger;
        private readonly IConfiguration _configuration;

        public HealthCheckController(ILogger<HealthCheckController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check endpoint called");
            
            var serviceUrls = new
            {
                AuthService = _configuration["ServiceUrls:AuthService"],
                PatientService = _configuration["ServiceUrls:PatientService"]
            };

            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Services = serviceUrls
            });
        }
    }
} 