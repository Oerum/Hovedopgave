using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Sellix.Application.Interfaces;

namespace Api.Sellix.Controllers
{
    [Route("API/Sellix/Order")]
    public class DiscordBotPurchaseController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISellixGatewayBuyHandlerImplementation _handler;
        private ILogger<DiscordBotPurchaseController> _logger;

        public DiscordBotPurchaseController(IConfiguration configuration ,ISellixGatewayBuyHandlerImplementation handler, ILogger<DiscordBotPurchaseController> logger)
        {
            _handler = handler;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("GrantLicenseOrder")]
        public async Task<IActionResult> PassToDb([FromBody] JsonObject json)
        {
            try
            {
                Request.Headers.TryGetValue("X-Sellix-Signature", out var token);

                // Log all headers
                var headers = Request.Headers.Select(header => $"{header.Key}: {header.Value}");
                _logger.LogInformation($"[POST REST - Purchase] Headers: {string.Join("\n", headers)}");
                _logger.LogInformation($"[POST REST - Purchase] X-Sellix-Signature: {token}");

                if (string.IsNullOrEmpty(token.ToString()))
                    return StatusCode(StatusCodes.Status401Unauthorized, "Invalid Token");

                var result = await _handler.OrderSubmit(json);

                if (result)
                    return Ok();

                return StatusCode(StatusCodes.Status500InternalServerError, "Serialization Failure");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
