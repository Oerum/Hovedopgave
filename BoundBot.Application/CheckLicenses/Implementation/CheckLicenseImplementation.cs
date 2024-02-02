using BoundBot.Application.CheckLicenses.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.CheckLicenses.Implementation;

public class CheckLicenseImplementation : ICheckLicenseImplementation
{
    private readonly ILogger<CheckLicenseImplementation> _logger;
    private readonly ICheckLicenseRepository _repository;
    private readonly IDomainRoleCheck _roleCheck;
    public CheckLicenseImplementation(ILogger<CheckLicenseImplementation> logger, ICheckLicenseRepository repository, IDomainRoleCheck roleCheck)
    {
        _logger = logger;
        _repository = repository;
        _roleCheck = roleCheck;
    }


    async Task ICheckLicenseImplementation.CheckLicense(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var access = await _roleCheck.RoleCheck(command, RoleCheckEnum.BoundAll);

            if (access)
            {
                await _repository.CheckLicense(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("1", ex);
        }
    }
}