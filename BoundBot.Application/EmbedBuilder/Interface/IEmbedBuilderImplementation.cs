using Crosscutting;
using Discord.WebSocket;

namespace BoundBot.Application.EmbedBuilder.Interface;

public interface IEmbedBuilderImplementation
{
    Task EmbedBuilder(SocketSlashCommand command, HttpClient client);
}