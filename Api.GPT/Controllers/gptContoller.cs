using Crosscutting;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Discord.Rest;
using gpt.application.Interface;
using Gpt.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.GPT.Controllers
{
    [Route("API/GPT")]
    public class gptContoller : Controller
    {
        private readonly ILogger<gptContoller> _logger;
        private readonly IConfiguration _configuration;
        private readonly IGptImplementation _implementation;

        public gptContoller(ILogger<gptContoller> logger, IConfiguration configuration, IGptImplementation implementation)
        {
            _logger = logger;
            _configuration = configuration;
            _implementation = implementation;
        }

        [Authorize(Policy = PolicyConfiguration.AdminPolicy)]
        [HttpPost("UpdateModel")]
        public async Task<IActionResult> UpdateModel([FromBody] GptModel model)
        {
            try
            {
                return Ok(await _implementation.UpdateFtModel(model.MessagesToCollect));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Update Model Error: {ErrorMessage}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = PolicyConfiguration.UserPolicy)]
        [HttpPost("AI")]
        public async Task<IActionResult> Gpt([FromBody] GptModel question)
        {
            try
            {
                if (question.Question != null)
                {
                    return Ok(await _implementation.Gpt(question.Question));
                }

                return Ok("Empty Request");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API AI Reply Error: {ErrorMessage}", ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
