using Crosscutting;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.DiscordBot.Controllers
{

    [Route("API/DiscordBot")]
    public class JsonWebTokensController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JsonWebTokensController> _logger;

        public JsonWebTokensController(IConfiguration configuration, ILogger<JsonWebTokensController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("JwtRefreshAndGenerate")]
        [AllowAnonymous]
        public async Task<IActionResult> Generate([FromBody] DiscordModelDto model)
        {
            try
            {
                var claims = PolicyClaimAuth.ClaimsConfiguration(_configuration, _logger, model);

                var jwt = await JwtApiResponse.JwtRefreshAndGenerate(claims, _configuration, model.RefreshToken, model.DiscordId);

                return await Task.FromResult(Ok(jwt));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unsuccessful DiscordBot JWT Generation: " + ex.Message);
            }
        }
    }
}
