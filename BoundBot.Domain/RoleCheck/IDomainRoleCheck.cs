using BoundBot.Components.RoleCheckEnum;
using Discord.WebSocket;

namespace BoundBot.Domain.RoleCheck;

public interface IDomainRoleCheck
{
    Task<bool> RoleCheck(SocketSlashCommand command, RoleCheckEnum role);
}