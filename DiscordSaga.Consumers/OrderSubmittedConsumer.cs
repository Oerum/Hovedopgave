using Auth.Database.Contexts;
using Crosscutting.SellixPayload;
using Crosscutting.TransactionHandling;
using Database.Application.Interface;
using DiscordSaga.Components.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;

namespace DiscordSaga.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmittedEvent>
{
    private readonly ILogger<OrderSubmittedConsumer> _logger;
    private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
    private readonly IMariaDbBackupImplementation _backup;
    private readonly ITopicProducer<LicenseGrantEvent> _producer;
    private readonly AuthDbContext _authDbContext;

    public OrderSubmittedConsumer(ILogger<OrderSubmittedConsumer> logger, IUnitOfWork<AuthDbContext> unitOfWork, IMariaDbBackupImplementation backup, ITopicProducer<LicenseGrantEvent> producer, AuthDbContext dbContext)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _backup = backup;
        _producer = producer;
        _authDbContext = dbContext;
    }
    public async Task Consume(ConsumeContext<OrderSubmittedEvent> context)
    {
        try
        {
            _logger.LogInformation("Consumer Received Order Submitted Event with CorrelationId: {id}", context.Message.CorrelationId);

            if (context.Message.Payload != null)
            {
                var deserializePayload =
                    JsonConvert.DeserializeObject<SellixPayloadNormal.Root>(context.Message.Payload);

                if (deserializePayload == null)
                    throw new NullReferenceException("OrderSubmitted Payload Null");

                await _unitOfWork.CreateTransaction(IsolationLevel.RepeatableRead);

                var exists = _authDbContext.Order.Where(x => x.UniqId == deserializePayload.Data.Uniqid);

                await _unitOfWork.Commit();

                if (exists.Any())
                {
                    _logger.LogInformation("Order Already Exists with UniqId: {UniqId}", deserializePayload.Data.Uniqid);
                    return;
                }

                await _producer.Produce(new LicenseGrantEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Payload = context.Message.Payload
                });

                _logger.LogInformation("Consumer Finished Order Submitted Event with CorrelationId: {CorrelationId}", context.Message.CorrelationId);

            }
            else
            {
                _logger.LogError("Consumer Failed Order Submitted Event with CorrelationId: {CorrelationId}", context.Message.CorrelationId);

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Order Submitted Consumer Failure!");
            throw new ConsumerException(ex.Message);
        }
    }
}