using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Sellix.Application.Interfaces;

namespace Api.Sellix.Controllers
{
    [Route("API/Sellix/Order")]
    public class DiscordBotPurchaseController : Controller
    {
        private readonly ISellixGatewayBuyHandlerImplementation _handler;
        public DiscordBotPurchaseController(ISellixGatewayBuyHandlerImplementation handler)
        {
            _handler = handler;
        }

        [HttpPost("GrantLicenseOrder")]
        public async Task<IActionResult> PassToDb([FromBody] JsonObject json)
        {
            try
            {
                var result = await _handler.OrderHandler(json);

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
