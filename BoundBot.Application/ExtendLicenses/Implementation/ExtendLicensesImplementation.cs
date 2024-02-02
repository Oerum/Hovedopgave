using BoundBot.Application.ExtendLicenses.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.ExtendLicenses.Implementation;

public class ExtendLicensesImplementation : IExtendLicensesImplementation
{
    private readonly ILogger<ExtendLicensesImplementation> _logger;
    private readonly IExtendLicensesRepository _repository;
    private readonly IDomainRoleCheck _roleCheck;

    public ExtendLicensesImplementation(ILogger<ExtendLicensesImplementation> logger, IExtendLicensesRepository repository, IDomainRoleCheck roleCheck)
    {
        _logger = logger;
        _repository = repository;
        _roleCheck = roleCheck;
    }


    async Task IExtendLicensesImplementation.Extend(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var access = await _roleCheck.RoleCheck(command, RoleCheckEnum.BoundMod);

            if (access)
            {
                await _repository.Extend(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("1", ex);
        }
    }
}