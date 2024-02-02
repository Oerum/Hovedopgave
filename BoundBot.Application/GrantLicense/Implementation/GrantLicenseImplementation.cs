using BoundBot.Application.GrantLicense.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.GrantLicense.Implementation;

public class GrantLicenseImplementation : IGrantLicenseImplementation
{
    private readonly ILogger<GrantLicenseImplementation> _logger;
    private readonly IDomainRoleCheck _roleCheck;
    private readonly IGrantLicenseRepository _grantLicenseRepository;

    public GrantLicenseImplementation(ILogger<GrantLicenseImplementation> logger, IDomainRoleCheck roleCheck, IGrantLicenseRepository grantLicenseRepository)
    {
        _logger = logger;
        _roleCheck = roleCheck;
        _grantLicenseRepository = grantLicenseRepository;
    }


    async Task IGrantLicenseImplementation.GrantLicense(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var access = await _roleCheck.RoleCheck(command, RoleCheckEnum.BoundMod);

            if (access)
            {
                await _grantLicenseRepository.GrantLicense(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("1", ex);
        }
    }
}