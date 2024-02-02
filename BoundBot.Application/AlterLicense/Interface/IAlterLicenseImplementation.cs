using Discord.WebSocket;

namespace BoundBot.Application.AlterLicense.Interface
{
    public interface IAlterLicenseImplementation
    {
        Task AlterLicense(SocketSlashCommand command, HttpClient client);
    }
}
