using Discord.WebSocket;

namespace BoundBot.Application.UpdateHwid.Interface;

public interface IUpdateHwidRepository
{
    Task UpdateHwid(SocketSlashCommand command, HttpClient client);
}