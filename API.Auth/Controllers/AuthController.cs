using Auth.Application.Interface;
using Crosscutting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Auth.Controllers
{
    [Route("/API/BC/Auth")]
    public class AuthController : Controller
    {
        private readonly IAuthImplementation _auth;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(IAuthImplementation auth, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _auth = auth;
            _configuration = configuration;
            _logger = logger;
        }

        [Authorize(Policy = "user")]
        [Authorize(Policy = "hwid")]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Auth([FromBody] AuthModelDTO model)
        {
            try
            {
                _logger.LogInformation("Auth Initiated by: {}", model.DiscordId);
                var result = await _auth.Auth(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
