using Discord.WebSocket;

namespace BoundBot.Application.StaffLicense.Interface;

public interface IStaffLicenseRepository
{
    Task StaffLicense(SocketSlashCommand command, HttpClient client);
}