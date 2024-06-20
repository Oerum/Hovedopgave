using BoundBot.Application.UpdateDiscord.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.UpdateDiscord.Implementation;

public class UpdateDiscordImplementation : IUpdateDiscordImplementation
{
    private readonly ILogger<UpdateDiscordImplementation> _logger;
    private readonly IUpdateDiscordRepository _repository;
    private readonly IDomainRoleCheck _roleCheck;

    public UpdateDiscordImplementation(ILogger<UpdateDiscordImplementation> logger, IUpdateDiscordRepository repository, IDomainRoleCheck roleCheck)
    {
        _logger = logger;
        _repository = repository;
        _roleCheck = roleCheck;
    }

    async Task IUpdateDiscordImplementation.UpdateDiscord(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var access = await _roleCheck.RoleCheck(command, RoleCheckEnum.BoundAll);

            if (access)
            {
                await _repository.UpdateDiscord(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateDiscordImplementation");
        }
    }
}