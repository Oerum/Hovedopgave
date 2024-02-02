using BoundBot.Application.StaffLicense.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.StaffLicense.Implementation;

public class StaffLicenseImplementation : IStaffLicenseImplementation
{
    private readonly ILogger<StaffLicenseImplementation> _logger;
    private readonly IDomainRoleCheck _domainRoleCheck;
    private readonly IStaffLicenseRepository _staffLicenseRepository;

    public StaffLicenseImplementation(ILogger<StaffLicenseImplementation> logger, IDomainRoleCheck domainRoleCheck, IStaffLicenseRepository staffLicenseRepository)
    {
        _logger = logger;
        _domainRoleCheck = domainRoleCheck;
        _staffLicenseRepository = staffLicenseRepository;
    }


    async Task IStaffLicenseImplementation.StaffLicense(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var access = await _domainRoleCheck.RoleCheck(command, RoleCheckEnum.BoundStaffAndAbove);

            if (access)
            {
                await _staffLicenseRepository.StaffLicense(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("1" , ex);
        }
    }
}