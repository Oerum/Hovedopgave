using Crosscutting;
using MassTransit;

namespace DiscordSaga.Components.Events;

public class OrderSubmittedEvent : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
    public string? Payload { get; set; }
}

public class LicenseGrantEvent : CorrelatedBy<Guid>
{
    public string? Payload { get; set; }
    public int? Quantity { get; set; }
    public DateTime Time { get; set; }
    public WhichSpec? WhichSpec { get; set; }
    public Guid CorrelationId { get; set; }
}

public class LicenseNotificationEvent : CorrelatedBy<Guid>
{
    public string? Payload { get; set; }
    public int? Quantity { get; set; }
    public DateTime Time { get; set; }
    public WhichSpec? WhichSpec { get; set; }
    public Guid CorrelationId { get; set; }
}

public class LicenseDatabaseBackupEvent : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
}


