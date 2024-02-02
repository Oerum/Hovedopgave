using BoundBot.Application.DatabaseBackup.Interface;
using BoundBot.Components.RoleCheckEnum;
using BoundBot.Domain.RoleCheck;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BoundBot.Application.DatabaseBackup.Implementation;

public class DatabaseBackupImplementation : IDatabaseBackupImplementation
{
    private readonly ILogger<DatabaseBackupImplementation> _logger;
    private readonly IDomainRoleCheck _roleCheck;
    private readonly IDatabaseBackupRepository _backupRepository;

    public DatabaseBackupImplementation(ILogger<DatabaseBackupImplementation> logger, IDomainRoleCheck roleCheck, IDatabaseBackupRepository backupRepository)
    {
        _logger = logger;
        _roleCheck = roleCheck;
        _backupRepository = backupRepository;
    }


    async Task IDatabaseBackupImplementation.DbBackup(SocketSlashCommand command, HttpClient client)
    {
        try
        {
            var access = await _roleCheck.RoleCheck(command, RoleCheckEnum.BoundMod);

            if (access)
            {
                await _backupRepository.DbBackup(command, client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("1", ex);
        }
    }
}