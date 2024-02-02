using Discord.WebSocket;

namespace BoundBot.Application.CheckLicenses.Interface;

public interface ICheckLicenseRepository
{
    Task CheckLicense(SocketSlashCommand command, HttpClient client);
}