using Discord.WebSocket;

namespace BoundBot.Application.GrantLicense.Interface;

public interface IGrantLicenseRepository
{
    Task GrantLicense(SocketSlashCommand command, HttpClient client);
}