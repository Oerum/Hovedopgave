using Discord.WebSocket;

namespace BoundBot.Application.Gpt.Interface;

public interface IGptDiscordRepository
{
    Task UpdateFtModel(SocketSlashCommand command, HttpClient client);
}