using BoundBot.Application.AlterLicense.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.AlterLicense.Implementation
{
    public class AlterLicenseImplementation : IAlterLicenseImplementation
    {
        private readonly ILogger<AlterLicenseImplementation> _logger;
        private readonly IAlterLicenseRepository _repository;
        private readonly IDomainRoleCheck _roleCheck;

        public AlterLicenseImplementation(IDomainRoleCheck roleCheck, IAlterLicenseRepository repository, ILogger<AlterLicenseImplementation> logger)
        {
            _roleCheck = roleCheck;
            _repository = repository;
            _logger = logger;
        }

        async Task IAlterLicenseImplementation.AlterLicense(SocketSlashCommand command, HttpClient client)
        {
            try
            {
                var access = await _roleCheck.RoleCheck(command, RoleCheckEnum.BoundAll);

                if (access)
                {
                    await _repository.AlterLicense(command, client);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("1", ex);
            }
        }
    }
}
