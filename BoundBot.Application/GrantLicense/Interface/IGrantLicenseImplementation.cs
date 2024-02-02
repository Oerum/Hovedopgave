using Discord.WebSocket;

namespace BoundBot.Application.GrantLicense.Interface;

public interface IGrantLicenseImplementation
{
    Task GrantLicense(SocketSlashCommand command, HttpClient client);
}