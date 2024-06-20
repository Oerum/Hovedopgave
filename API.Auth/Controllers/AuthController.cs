using Auth.Application.Interface;
using Auth.Components;
using Auth.Domain;
using Crosscutting;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace API.Auth.Controllers
{
    [Route("API/Core/Auth")]
    public class AuthController : Controller
    {
        private readonly IAuthImplementation _auth;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthDomain _authDomain;

        public AuthController(IAuthImplementation auth, IConfiguration configuration, ILogger<AuthController> logger, IAuthDomain authDomain)
        {
            _auth = auth;
            _configuration = configuration;
            _logger = logger;
            _authDomain = authDomain;
        }

        [Authorize(Policy = PolicyConfiguration.AuthUser)]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Auth([FromBody] AuthModelDTO model)
        {
            try
            {
                _logger.LogInformation($"Auth Initiated by: {model.Firstname} {model.Lastname} | {model.DiscordUsername} : {model.DiscordId} | {model.Email}");

                var result = await _auth.Auth(model);

                var success = await _authDomain.AuthenticateAsync(result);

                if (!success)
                {
                    return StatusCode(401, result.Where(x=>x.Success == false));
                }
               
                return Ok(result.Where(x => x.Success == true));
                
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Policy = PolicyConfiguration.AuthUser)]
        [HttpPost("DiscordOAuth")]
        public async Task<IActionResult> DiscordOAuth([FromBody] DiscordOAuthDTO model)
        {
            try
            {
                _logger.LogInformation($"DiscordOAuth Initiated by: {model.State} at: " + DateTime.UtcNow);

                var result = await _auth.DiscordOAuth(model);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }       
    }
}
