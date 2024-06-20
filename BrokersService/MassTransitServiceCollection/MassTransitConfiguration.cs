using Auth.Database.Model;
using Confluent.Kafka;
using Crosscutting.TransactionHandling;
using Database.Application.Implementation;
using Database.Application.Interface;
using Database.Infrastructure;
using DiscordBot.Application.Interface;
using DiscordBot.Infrastructure;
using DiscordSaga;
using DiscordSaga.Components.Events;
using DiscordSaga.Components.StateMachineInstance;
using DiscordSaga.Consumers;
using LoggingService.Components.SerilogConfiguration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver.Core.Configuration;
using Sellix.Application.Implementation;
using Sellix.Application.Interfaces;
using Sellix.Infrastructure;
using Serilog;
using Serilog.AspNetCore;
using StackExchange.Redis;
using System.Reflection;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Auth.Database.Contexts;

namespace BrokersService.MassTransitServiceCollection;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransitWithRabbitMqAndKafka(this IServiceCollection services, IConfiguration configuration, bool registerBrokerHandler = false)
    {
        services.AddMassTransit(x =>
        {
            var test = x.AddLogging(l =>
            {
                l.AddLoggerConfig(configuration);
            });

            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitMQ", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:User"]);
                    h.Password(configuration["RabbitMQ:Pass"]);
                });

                cfg.ConfigureEndpoints(context);
            });

            x.TryAddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
            x.TryAddScoped<ISellixGatewayBuyHandlerRepository, SellixGatewayBuyHandlerRepository>();
            x.TryAddScoped<IDiscordBotNotificationRepository, DiscordBotNotificationRepository>();
            x.TryAddScoped<IMariaDbBackupImplementation, MariaDbBackupImplementation>();
            x.TryAddScoped<IMariaDbBackupRepository, MariaDbBackupRepository>();
            x.TryAddScoped<ISellixGatewayBuyHandlerRepository, SellixGatewayBuyHandlerRepository>();
            x.TryAddScoped<ISellixGatewayBuyHandlerImplementation, SellixGatewayBuyHandlerImplementation>();
            x.TryAddScoped<IDiscordConnectionHandler, DiscordConnectionHandler>();

            x.AddRider(r =>
            {
                r.AddProducer<OrderSubmittedEvent>("Discord-Order-Submitted");
                r.AddProducer<LicenseGrantEvent>("Discord-License-Grant");
                r.AddProducer<LicenseNotificationEvent>("Discord-Notification");
                r.AddProducer<LicenseDatabaseBackupEvent>("Discord-Database-BackupEvent");

                r.AddConsumer<OrderSubmittedConsumer>();
                r.AddConsumer<LicenseGrantEventConsumer>();
                r.AddConsumer<LicenseNotificationEventConsumer>();
                r.AddConsumer<LicenseDatabaseBackupEventConsumer>();

                r.AddSagaStateMachine<DiscordStateMachine, SagaDiscord>().InMemoryRepository(); //.MongoDbRepository(m =>
                //{
                //    m.Connection =
                //        $"mongodb://{configuration["MONGO:USER"]}:{configuration["MONGO:PASSWORD"]}@mongoDB:27017";
                //    m.DatabaseName = "licensedb";
                //    m.CollectionName = "license";
                //});

                r.UsingKafka((context, cfg) =>
                {
                    cfg.Host("kafka");

                    #region Discord-Order-Submitted
                    cfg.TopicEndpoint<Null, OrderSubmittedEvent>("Discord-Order-Submitted", "Discord", e =>
                    {
                        e.AutoOffsetReset = AutoOffsetReset.Earliest;
                        e.ConcurrentConsumerLimit = 5;
                        e.ConcurrentDeliveryLimit = 1;
                        e.ConcurrentMessageLimit = 3;
                        e.UseMessageRetry(retry => retry.Interval(5, 1000));
                        e.CreateIfMissing(m =>
                        {
                            m.NumPartitions = 10;
                        });
                        e.UseNewtonsoftJsonSerializer();
                        e.UseNewtonsoftJsonDeserializer();
                        e.ConfigureSaga<SagaDiscord>(context);
                        e.ConfigureConsumer<OrderSubmittedConsumer>(context);
                    });
                    #endregion
                    #region Discord-License-Grant
                    cfg.TopicEndpoint<Null, LicenseGrantEvent>("Discord-License-Grant", "Discord", e =>
                    {
                        e.AutoOffsetReset = AutoOffsetReset.Earliest;
                        e.ConcurrentConsumerLimit = 5;
                        e.ConcurrentDeliveryLimit = 1;
                        e.ConcurrentMessageLimit = 3;
                        e.PrefetchCount = 10000;
                        e.UseMessageRetry(retry => retry.Interval(5, 1000));
                        e.CreateIfMissing(m =>
                        {
                            m.NumPartitions = 10;
                        });
                        e.UseNewtonsoftJsonSerializer();
                        e.UseNewtonsoftJsonDeserializer();
                        e.ConfigureSaga<SagaDiscord>(context);
                        e.ConfigureConsumer<LicenseGrantEventConsumer>(context);
                    });
                    #endregion
                    #region Discord-Notification
                    cfg.TopicEndpoint<Null, LicenseNotificationEvent>("Discord-Notification", "Discord", e =>
                    {
                        e.AutoOffsetReset = AutoOffsetReset.Earliest;
                        e.ConcurrentConsumerLimit = 5;
                        e.ConcurrentDeliveryLimit = 1;
                        e.ConcurrentMessageLimit = 3;
                        e.UseMessageRetry(retry => { 
                            retry.Interval(5, 5000);
                        });
                        e.CreateIfMissing(m =>
                        {
                            m.NumPartitions = 10;
                        });
                        e.UseNewtonsoftJsonSerializer();
                        e.UseNewtonsoftJsonDeserializer();
                        e.ConfigureSaga<SagaDiscord>(context);
                        e.ConfigureConsumer<LicenseNotificationEventConsumer>(context);
                    });
                    #endregion
                    #region Discord-Database-BackupEvent
                    cfg.TopicEndpoint<Null, LicenseDatabaseBackupEvent>("Discord-Database-BackupEvent", "Discord", e =>
                    {
                        e.AutoOffsetReset = AutoOffsetReset.Earliest;
                        e.ConcurrentConsumerLimit = 5;
                        e.ConcurrentDeliveryLimit = 1;
                        e.ConcurrentMessageLimit = 3;
                        e.UseMessageRetry(retry => retry.Interval(5, 1000));
                        e.CreateIfMissing(m =>
                        {
                            m.NumPartitions = 10;
                        });
                        e.UseNewtonsoftJsonSerializer();
                        e.UseNewtonsoftJsonDeserializer();
                        e.ConfigureSaga<SagaDiscord>(context);
                        e.ConfigureConsumer<LicenseDatabaseBackupEventConsumer>(context);
                    });
                    #endregion
                });
            });
        });

        return services;
    }
}