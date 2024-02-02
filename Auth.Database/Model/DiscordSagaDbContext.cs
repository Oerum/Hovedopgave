using DiscordSaga.Components.StateMachineInstance;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Database.Model;

public class StateMap :
    SagaClassMap<SagaDiscord>
{
    protected override void Configure(EntityTypeBuilder<SagaDiscord> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        // If using Optimistic concurrency, otherwise remove this property
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}

public class DiscordSagaDbContext :
    SagaDbContext
{
    public DiscordSagaDbContext(DbContextOptions<DiscordSagaDbContext> options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new StateMap(); }
    }
}