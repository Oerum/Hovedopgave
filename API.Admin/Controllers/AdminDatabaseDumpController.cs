using Crosscutting.Configuration.AuthPolicyConfiguration;
using Database.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Admin.Controllers
{
    [Route("API/Core/Admin/")]
    public class AdminDatabaseDumpController : Controller
    {
        private readonly IMariaDbBackupImplementation _backup;

        public AdminDatabaseDumpController(IMariaDbBackupImplementation backup)
        {
            _backup = backup;
        }

        [Authorize(Policy = PolicyConfiguration.AdminPolicy)]
        [HttpGet("DbDump")]
        public async Task<IActionResult> DatabaseDump()
        {
            try
            {
                await _backup.Backup();
                return Ok("Dump Successful");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
