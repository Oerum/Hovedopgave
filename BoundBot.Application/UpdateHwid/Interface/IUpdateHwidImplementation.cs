using Discord.WebSocket;

namespace BoundBot.Application.UpdateHwid.Interface;

public interface IUpdateHwidImplementation
{
    Task UpdateHwid(SocketSlashCommand command, HttpClient client);
}