using Discord.WebSocket;

namespace BoundBot.Application.ExtendLicenses.Interface;

public interface IExtendLicensesRepository
{
    Task Extend(SocketSlashCommand command, HttpClient client);
}