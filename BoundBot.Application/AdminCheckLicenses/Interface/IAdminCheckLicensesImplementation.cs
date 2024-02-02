using Discord.WebSocket;

namespace BoundBot.Application.AdminCheckLicenses.Interface;

public interface IAdminCheckLicensesImplementation
{
    Task CheckLicenses(SocketSlashCommand command, HttpClient client);
}