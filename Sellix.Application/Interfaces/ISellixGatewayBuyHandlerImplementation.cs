using System.Text.Json.Nodes;

namespace Sellix.Application.Interfaces
{
    public interface ISellixGatewayBuyHandlerImplementation
    {
        Task<bool> OrderSubmit(JsonObject root);
    }
}
