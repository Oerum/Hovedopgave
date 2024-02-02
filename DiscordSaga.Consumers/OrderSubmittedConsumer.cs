using Auth.Database;
using Crosscutting.TransactionHandling;
using Database.Application.Interface;
using DiscordSaga.Components.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DiscordSaga.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmittedEvent>
{
    private readonly ILogger<OrderSubmittedConsumer> _logger;
    private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
    private readonly IMariaDbBackupImplementation _backup;
    private readonly ITopicProducer<LicenseGrantEvent> _producer;

    public OrderSubmittedConsumer(ILogger<OrderSubmittedConsumer> logger, IUnitOfWork<AuthDbContext> unitOfWork, IMariaDbBackupImplementation backup, ITopicProducer<LicenseGrantEvent> producer)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _backup = backup;
        _producer = producer;
    }
    public async Task Consume(ConsumeContext<OrderSubmittedEvent> context)
    {
        try
        {
            _logger.LogInformation("Consumer Received Order Submitted Event with CorrelationId: {id}", context.Message.CorrelationId);

            await _producer.Produce(new LicenseGrantEvent
            {
                CorrelationId = context.Message.CorrelationId,
                Payload = context.Message.Payload
            });

            _logger.LogInformation("Consumer Finished Order Submitted Event with CorrelationId: {CorrelationId}", context.Message.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database Backup Consumer Error");
            throw new ConsumerException(ex.Message);
        }
    }
}