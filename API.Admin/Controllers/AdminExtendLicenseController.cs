using Admin.Application.Interface.ExtendLicense;
using Crosscutting;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Admin.Controllers
{
    [Route("API/Core/Admin/")]
    public class AdminExtendLicenseController : Controller
    {
        private readonly IAdminExtendLicensesImplementation _license;
        public AdminExtendLicenseController(IAdminExtendLicensesImplementation license)
        {
            _license = license;
        }

        [Authorize(Policy = PolicyConfiguration.AdminOrStaff)]
        [HttpPost("ExtendLicenses")]
        public async Task<IActionResult> ExtendLicenses([FromBody] ExtendLicenseDto model)
        {
            try
            {
                int minutesToExtend = Convert.ToInt32(model.MinutesToExtend);
                string? discordId = model.DiscordId;

                var result = await _license.ExtendLicense(minutesToExtend, discordId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
