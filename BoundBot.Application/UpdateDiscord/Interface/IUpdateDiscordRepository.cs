using Discord.WebSocket;

namespace BoundBot.Application.UpdateDiscord.Interface;

public interface IUpdateDiscordRepository
{
    Task UpdateDiscord(SocketSlashCommand command, HttpClient client);
}