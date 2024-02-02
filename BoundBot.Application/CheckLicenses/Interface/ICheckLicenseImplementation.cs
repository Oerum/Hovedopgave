using Discord.WebSocket;

namespace BoundBot.Application.CheckLicenses.Interface;

public interface ICheckLicenseImplementation
{
    Task CheckLicense(SocketSlashCommand command, HttpClient client);
}