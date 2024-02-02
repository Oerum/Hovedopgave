using System.Text.Json.Nodes;
using DiscordSaga.Components.Events;
using MassTransit;
using Sellix.Application.Interfaces;

namespace Sellix.Application.Implementation
{
    public class SellixGatewayBuyHandlerImplementation : ISellixGatewayBuyHandlerImplementation
    {
        private readonly ITopicProducer<OrderSubmittedEvent> _topicProducer;


        public SellixGatewayBuyHandlerImplementation(ITopicProducer<OrderSubmittedEvent> topicProducer)
        {
            _topicProducer = topicProducer;
        }

        async Task<bool> ISellixGatewayBuyHandlerImplementation.OrderHandler(JsonObject root)
        {
            try
            {
                await _topicProducer.Produce(new OrderSubmittedEvent
                {
                    CorrelationId = Guid.NewGuid(),
                    Payload = root.ToJsonString()
                });

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
