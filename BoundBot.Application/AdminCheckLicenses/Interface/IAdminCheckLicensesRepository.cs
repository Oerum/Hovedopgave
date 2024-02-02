using Discord.WebSocket;

namespace BoundBot.Application.AdminCheckLicenses.Interface;

public interface IAdminCheckLicensesRepository
{
    Task CheckLicenses(SocketSlashCommand command, HttpClient client);
}