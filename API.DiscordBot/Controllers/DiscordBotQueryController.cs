using Crosscutting.Configuration.AuthPolicyConfiguration;
using DiscordBot.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.DiscordBot.Controllers
{
    [Route("API/DiscordBot/Query")]
    public class DiscordBotQueryController : Controller
    {
        private readonly IDiscordBotQueryImplementation _discord;
        private readonly IConfiguration _configuration;

        public DiscordBotQueryController(IDiscordBotQueryImplementation discord, IConfiguration configuration)
        {
            _discord = discord;
            _configuration = configuration;
        }

        [Authorize(Policy = PolicyConfiguration.AdminOrStaff)]
        [HttpGet("CheckDB/{username}/{id}")]
        public async Task<IActionResult> CheckDb(string username, string id)
        {
            try
            {
                var result = await _discord.CheckDB(username, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = PolicyConfiguration.UserPolicy)]
        [HttpGet("CheckMe/{username}/{id}")]
        public async Task<IActionResult> CheckMe(string username, string id)
        {
            try
            {
                var result = await _discord.CheckDB(username, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = PolicyConfiguration.UserPolicy)]
        [HttpGet("OAuth/client/secret")]
        public IActionResult GetClientSecret()
        {
            try
            {
                var result = _configuration["Discord:Secret"];
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = PolicyConfiguration.UserPolicy)]
        [HttpGet("OAuth/client/id")]
        public IActionResult GetClientId()
        {
            try
            {
                var result = _configuration["Discord:Id"];
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = PolicyConfiguration.UserPolicy)]
        [HttpGet("OAuth/client/encode/key")]
        public IActionResult GetEncodingKey()
        {
            try
            {
                var result = _configuration["Encoding:Key"];
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
