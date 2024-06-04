using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSA.Common.Contracts.Settings;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.Common.Settings;

namespace MSA.Common.PostgresMassTransit.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection services)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumers(Assembly.GetEntryAssembly());
            config.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                var serviceSetting = configuration.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();
                RabbitMQSetting rabbitMQSetting = configuration.GetSection(nameof(RabbitMQSetting)).Get<RabbitMQSetting>();
                configurator.Host(rabbitMQSetting.Host);
                configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSetting.ServiceName, false));
                configurator.UseMessageRetry(policy =>
                {
                    policy.Interval(3, TimeSpan.FromSeconds(3));
                });
            });
        });
        return services;
    }
    public static IServiceCollection AddMassTransitWithPostgresOutbox<TDbContext>(
        this IServiceCollection services,
        Action<IBusRegistrationConfigurator> furtherConfig = null)
        where TDbContext : AppDbContextBase
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());

            configure.UsingRabbitMq((context, configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                var serviceSetting = configuration.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();
                RabbitMQSetting rabitMQSetting = configuration.GetSection(nameof(RabbitMQSetting)).Get<RabbitMQSetting>();
                configurator.Host(rabitMQSetting.Host);
                configurator.ConfigureEndpoints(context,
                    new KebabCaseEndpointNameFormatter(serviceSetting.ServiceName, false));
                configurator.UseMessageRetry(retryPoilicy =>
                {
                    retryPoilicy.Interval(3, TimeSpan.FromSeconds(10));
                });
            });

            configure.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            furtherConfig?.Invoke(configure);
        });

        return services;
    }
}