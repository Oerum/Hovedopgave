using Crosscutting.SellixPayload;
using DiscordSaga.Components.Events;

namespace Sellix.Application.Interfaces
{
    public interface ISellixGatewayBuyHandlerRepository
    {
        Task<LicenseNotificationEvent> OrderHandler(SellixPayloadNormal.Root root);
    }
}
