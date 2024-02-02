using Discord.WebSocket;

namespace BoundBot.Application.Gpt.Interface;

public interface IGptDiscordImplementation
{
    Task UpdateFtModel(SocketSlashCommand command, HttpClient client);
}