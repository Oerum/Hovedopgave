using System.Data;
using Auth.Database.Contexts;
using Crosscutting.SellixPayload;
using Crosscutting.TransactionHandling;
using Database.Application.Interface;
using DiscordSaga.Components.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DiscordSaga.Consumers;

public class LicenseDatabaseBackupEventConsumer : IConsumer<LicenseDatabaseBackupEvent>
{
    private readonly ILogger<LicenseDatabaseBackupEventConsumer> _logger;
    private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
    private readonly IMariaDbBackupImplementation _backup;

    public LicenseDatabaseBackupEventConsumer(ILogger<LicenseDatabaseBackupEventConsumer> logger, IUnitOfWork<AuthDbContext> unitOfWork, IMariaDbBackupImplementation backup)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _backup = backup;
    }
    public async Task Consume(ConsumeContext<LicenseDatabaseBackupEvent> context)
    {
        try
        {
            _logger.LogInformation("Consumer Received Backup Event with CorrelationId: {id}", context.Message.CorrelationId);

            await _backup.Backup();

            _logger.LogInformation("Consumer Finished Backup Event with CorrelationId: {CorrelationId}", context.Message.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database Backup Consumer Error");

            throw new ConsumerException(ex.Message);
        }
    }
}