using Admin.Application.Interface.ExtendLicense;
using Admin.Application.Interface.GrantLicense;
using Crosscutting;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Admin.Controllers
{
    [Route("API/Core/Admin/")]
    public class AdminGrantLicenseController : Controller
    {
        private readonly IAdminGrantLicenseImplementation _alter;
        private readonly ILogger<AdminAlterLicenseController> _logger;
        public AdminGrantLicenseController(IAdminExtendLicensesImplementation license, IAdminGrantLicenseImplementation grant, ILogger<AdminAlterLicenseController> logger)
        {
            _alter = grant;
            _logger = logger;
        }


        [Authorize(Policy = PolicyConfiguration.AdminOrStaff)]
        [HttpPost("GrantLicense")]
        public async Task<IActionResult> ExtendLicenses([FromBody] GrantLicenseDto model)
        {
            try
            {
                var result = await _alter.GrantLicense(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
