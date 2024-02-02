using Discord.WebSocket;

namespace BoundBot.Application.ExtendLicenses.Interface;

public interface IExtendLicensesImplementation
{
    Task Extend(SocketSlashCommand command, HttpClient client);
}