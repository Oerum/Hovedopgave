using Discord.WebSocket;

namespace BoundBot.Application.StaffLicense.Interface;

public interface IStaffLicenseImplementation
{
    Task StaffLicense(SocketSlashCommand command, HttpClient client);
}