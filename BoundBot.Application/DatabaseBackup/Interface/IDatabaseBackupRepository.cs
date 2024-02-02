using Discord.WebSocket;

namespace BoundBot.Application.DatabaseBackup.Interface;

public interface IDatabaseBackupRepository
{
    Task DbBackup(SocketSlashCommand command, HttpClient client);
}