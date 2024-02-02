using Discord.WebSocket;

namespace BoundBot.Application.DatabaseBackup.Interface;

public interface IDatabaseBackupImplementation
{
    Task DbBackup(SocketSlashCommand command, HttpClient client);
}