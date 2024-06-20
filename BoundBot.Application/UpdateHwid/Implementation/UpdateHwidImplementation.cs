using System.Security.AccessControl;
using BoundBot.Application.UpdateHwid.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.UpdateHwid.Implementation;

public class UpdateHwidImplementation : IUpdateHwidImplementation
{
    private readonly ILogger<UpdateHwidImplementation> _logger;
    private readonly IUpdateHwidRepository _updateHwidRepository;
    private readonly IDomainRoleCheck _roleCheck;

    public UpdateHwidImplementation(IDomainRoleCheck roleCheck, ILogger<UpdateHwidImplementation> logger, IUpdateHwidRepository updateHwidRepository)
    {
        _roleCheck = roleCheck;
        _logger = logger;
        _updateHwidRepository = updateHwidRepository;
    }


    async Task IUpdateHwidImplementation.UpdateHwid(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var access = await _roleCheck.RoleCheck(command, RoleCheckEnum.BoundAll);

            if (access)
            {
                await _updateHwidRepository.UpdateHwid(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateHwidImplementation");
        }
    }
}