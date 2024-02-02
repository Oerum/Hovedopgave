﻿using DiscordBot.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.DiscordBot.Controllers
{
    [Route("API/DiscordBot/Query")]
    public class DiscordBotQueryController : Controller
    {
        private readonly IDiscordBotQueryImplementation _discord;
        public DiscordBotQueryController(IDiscordBotQueryImplementation discord)
        {
            _discord = discord;
        }

        [Authorize(Policy = "admin")]
        [Authorize(Policy = "staff")]
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

        [Authorize(Policy = "user")]
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
    }
}
