using Auth.Database;
using Crosscutting.SellixPayload;
using Crosscutting.TransactionHandling;
using DiscordSaga.Components.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using Sellix.Application.Interfaces;

namespace DiscordSaga.Consumers;

public class LicenseGrantEventConsumer : IConsumer<LicenseGrantEvent>
{
    private readonly ILogger<LicenseGrantEventConsumer> _logger;
    private readonly IUnitOfWork<AuthDbContext> _unitOfWork;
    private readonly ISellixGatewayBuyHandlerRepository _license;
    private readonly ITopicProducer<LicenseNotificationEvent> _producer;

    public LicenseGrantEventConsumer(ILogger<LicenseGrantEventConsumer> logger, IUnitOfWork<AuthDbContext> unitOfWork, ISellixGatewayBuyHandlerRepository license, ITopicProducer<LicenseNotificationEvent> producer)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _license = license;
        _producer = producer;
    }
    public async Task Consume(ConsumeContext<LicenseGrantEvent> context)
    {
        try
        {
            _logger.LogInformation("Consumer Received Grant License Event: {id} with payload: {Payload}", context.Message.CorrelationId, context.Message.Payload);

            await _unitOfWork.CreateTransaction(IsolationLevel.Serializable);
            if (context.Message.Payload != null)
            {
                var deserializePayload =
                    JsonConvert.DeserializeObject<SellixPayloadNormal.Root>(context.Message.Payload);

                var license = await _license.OrderHandler(deserializePayload ?? throw new NullReferenceException("LicenseGrant Payload Null"));

                await _unitOfWork.Commit();

                await _producer.Produce(new LicenseNotificationEvent
                {
                    Payload = license.Payload,
                    Quantity = license.Quantity,
                    Time = license.Time,
                    WhichSpec = license.WhichSpec,
                    CorrelationId = context.Message.CorrelationId
                });

                _logger.LogInformation("Consumer Finished Grant License Event with CorrelationId: {CorrelationId}", context.Message.CorrelationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "License Consumer Error");
            throw new ConsumerException(ex.Message);
        }
    }
}