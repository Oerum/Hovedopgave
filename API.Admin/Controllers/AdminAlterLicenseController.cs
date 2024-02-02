using Admin.Application.Interface.AlterLicense;
using Admin.Application.Interface.ExtendLicense;
using Admin.Application.Interface.GrantLicense;
using Crosscutting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Admin.Controllers
{
    [Route("API/BC/Admin/")]
    public class AdminAlterLicenseController : Controller
    {
        private readonly IAlterLicenseImplementation _alterLicense;
        private readonly ILogger<AdminAlterLicenseController> _logger;
        public AdminAlterLicenseController(ILogger<AdminAlterLicenseController> logger, IAlterLicenseImplementation alterLicense)
        {
            _logger = logger;
            _alterLicense = alterLicense;
        }

        [Authorize(Policy = "admin")]
        [Authorize(Policy = "staff")]
        [HttpPut("AlterLicense")]
        public async Task<IActionResult> AlterLicenses([FromBody] AlterLicenseDTO model)
        {
            try
            {
                var result = await _alterLicense.AlterLicense(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
