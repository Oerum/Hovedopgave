using System.Text.Json.Nodes;
using Crosscutting;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using DiscordBot.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.DiscordBot.Controllers
{
    [Route("API/DiscordBot/Command")]
    public class DiscordBotCommandController : Controller
    {
        private readonly IDiscordBotCommandImplementation _discord;

        public DiscordBotCommandController(IDiscordBotCommandImplementation discord)
        {
            _discord = discord;
        }

        [Authorize(Policy = PolicyConfiguration.AdminOrStaff)]
        [HttpPost("StaffLicense")]
        public async Task<IActionResult> StaffLicense([FromBody] DiscordModelDto model)
        {
            try
            {
                var result = await _discord.GetStaffLicense(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = PolicyConfiguration.UserPolicy)]
        [HttpPut("UpdateDiscord")]
        public async Task<IActionResult> UpdateDiscordAndRole([FromBody] DiscordModelDto model)
        {
            try
            {
                var result = await _discord.UpdateDiscordAndRole(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = PolicyConfiguration.UserPolicy)]
        [HttpPut("UpdateHwid")]
        public async Task<IActionResult> UpdateHwid([FromBody] DiscordModelDto model)
        {
            try
            {
                var result = await _discord.UpdateHwid(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
