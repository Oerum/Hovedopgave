using Crosscutting;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.GPT.Controllers
{

    [Route("API/GPT")]
    public class JsonWebTokensController : Controller
    {
        private readonly IConfiguration _configuration;

        public JsonWebTokensController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("JwtRefreshAndGenerate")]
        [AllowAnonymous]
        public async Task<IActionResult> Generate([FromBody] DiscordModelDto model)
        {
            try
            {
                var claims = PolicyClaimAuth.ClaimsConfiguration(_configuration, model);

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
