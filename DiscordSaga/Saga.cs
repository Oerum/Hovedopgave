using DiscordSaga.Components.Events;
using DiscordSaga.Components.StateMachineInstance;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DiscordSaga;

public class DiscordStateMachine : MassTransitStateMachine<SagaDiscord>
{
    private readonly ILogger<DiscordStateMachine> _logger;
    public State GrantLicenseState { get; set; }
    public State NotificationReadyState { get; set; }
    public State BackupReadyState { get; set; }

    public Event<OrderSubmittedEvent> OrderSubmittedEvent { get; set; }
    public Event<LicenseGrantEvent> GrantLicenseEvent { get; set; }
    public Event<LicenseNotificationEvent> NotifyEvent { get; set; }
    public Event<LicenseDatabaseBackupEvent> BackupEvent { get; set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DiscordStateMachine(ILogger<DiscordStateMachine> logger)
    {
        _logger = logger;

        Event(() => OrderSubmittedEvent, x => { x.CorrelateById(context => context.Message.CorrelationId);});
        Event(() => GrantLicenseEvent, x => { x.CorrelateById(context => context.Message.CorrelationId);});
        Event(() => NotifyEvent, x => { x.CorrelateById(context => context.Message.CorrelationId);});
        Event(() => BackupEvent, x => { x.CorrelateById(context => context.Message.CorrelationId);});

        InstanceState(x => x.CurrentState, GrantLicenseState, NotificationReadyState, BackupReadyState);

        Initially(
            When(OrderSubmittedEvent)
            .Then(context =>
            {
                _logger.LogInformation(1, "Saga Order Submitted Event: {0}", context.Message.CorrelationId);
                context.Saga.CorrelationId = context.Message.CorrelationId;
                context.Saga.Payload = context.Message.Payload;
            })
            .Then(context => _logger.LogInformation(1, "Saga Order Submitted Event [License Grant Event Produced]: {0}", context.Message.CorrelationId))
            .TransitionTo(GrantLicenseState));

        DuringAny(
            When(GrantLicenseEvent)
                .Then(context =>
                {
                    _logger.LogInformation(1, "Saga License Grant Event: {0}", context.Message.CorrelationId);
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.Payload = context.Message.Payload;
                    context.Saga.Quantity = context.Message.Quantity;
                    context.Saga.Time = context.Message.Time;
                    context.Saga.WhichSpec = context.Message.WhichSpec;
                })
                .Then(context => _logger.LogInformation(1, "Saga License Grant Event [License Notification Event Produced]: {0}", context.Message.CorrelationId))
                .TransitionTo(NotificationReadyState),

            When(NotifyEvent)
                .Then(context =>
                {
                    _logger.LogInformation(1, "Saga Notify Event: {0}\n{1}\n{2}\n{3}", context.Message.CorrelationId,
                        context.Message.Quantity, context.Message.Time, context.Message.WhichSpec);

                    context.Saga.CorrelationId = context.Message.CorrelationId;
                })
                .Then(context => _logger.LogInformation(1, "Saga Notify Event [Backup Event Produced]: {0}", context.Message.CorrelationId))
                .TransitionTo(BackupReadyState),

            When(BackupEvent)
                .Then(context =>
                {
                    _logger.LogInformation(1, "Saga Backup Event: {0}", context.Message.CorrelationId);
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                })
                .Finalize());


        SetCompletedWhenFinalized();
    }
}



