using Crosscutting;
using Discord.WebSocket;

namespace BoundBot.Application.EmbedBuilder.Interface;

public interface IEmbedBuilderRepository
{
    Task EmbedBuilder(SocketSlashCommand command, HttpClient client);
}