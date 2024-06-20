using System.Text.Json.Nodes;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sellix.Application.Interfaces;

namespace Api.Sellix.Controllers
{
    [Route("API/Sellix/Command")]
    public class SellixCommands : Controller
    {
        private readonly ISellixCouponCreateImplementation _couponCreateImplementation;

        public SellixCommands(ISellixCouponCreateImplementation couponCreateImplementation)
        {
            _couponCreateImplementation = couponCreateImplementation;
        }

        [Authorize(PolicyConfiguration.BoosterPolicy)]
        [HttpPost("CreateCoupon")]
        public async Task<IActionResult> Index([FromBody] JsonObject body)
        {
            try
            {
                var discordId = body["discord_id"]!.ToString();
                return Ok(await _couponCreateImplementation.CreateCoupon(discordId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
