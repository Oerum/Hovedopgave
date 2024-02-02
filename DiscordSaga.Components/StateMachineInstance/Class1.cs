using Crosscutting;
using MassTransit;

namespace DiscordSaga.Components.StateMachineInstance;

public class SagaDiscord : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string? Payload { get; set; }
    public int? Quantity { get; set; }
    public DateTime Time { get; set; }
    public WhichSpec? WhichSpec { get; set; }
    public int CurrentState { get; set; }
    public int Version { get; set; }
    // If using Optimistic concurrency, this property is required
    public required byte[] RowVersion { get; set; }
}
