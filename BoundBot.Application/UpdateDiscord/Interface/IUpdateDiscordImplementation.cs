using Discord.WebSocket;

namespace BoundBot.Application.UpdateDiscord.Interface;

public interface IUpdateDiscordImplementation
{
    Task UpdateDiscord(SocketSlashCommand command, HttpClient client);
}