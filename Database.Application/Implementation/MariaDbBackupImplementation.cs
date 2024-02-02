using Database.Application.Interface;
using Microsoft.Extensions.Logging;

namespace Database.Application.Implementation;

public class MariaDbBackupImplementation : IMariaDbBackupImplementation
{
    private readonly IMariaDbBackupRepository _repository;
    private readonly ILogger<MariaDbBackupImplementation> _logger;

    public MariaDbBackupImplementation(IMariaDbBackupRepository repository, ILogger<MariaDbBackupImplementation> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Backup()
    {
        try
        {
            await _repository.Backup();
        }
        catch (Exception ex)
        {
            _logger.LogError("Unsuccessful Backup: " + ex.Message);
        }
    }
}