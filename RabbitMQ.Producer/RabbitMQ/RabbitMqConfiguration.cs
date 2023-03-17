using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Producer.RabbitMQ.Client;
using RabbitMQ.Client;

namespace RabbitMQ.Producer.RabbitMQ;

internal static class RabbitMqConfiguration
{
    private const string SECTION_NAME = "rabbitmq";

    internal static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = new RabbitMqOptions();
        configuration.GetSection(SECTION_NAME).Bind(rabbitMqOptions);

        if (rabbitMqOptions.HostNames is null || !rabbitMqOptions.HostNames.Any())
        {
            throw new ArgumentException("RabbitMQ hostnames are not specified.", nameof(rabbitMqOptions.HostNames));
        }

        services.AddSingleton<IRabbitMqClient, RabbitMqClient>();

        var connectionFactory = new ConnectionFactory
        {
            Port = rabbitMqOptions.Port,
            VirtualHost = rabbitMqOptions.VirtualHost,
            UserName = rabbitMqOptions.Username,
            Password = rabbitMqOptions.Password,
            DispatchConsumersAsync = true,
        };

        services.AddSingleton(connectionFactory.CreateConnection(rabbitMqOptions.HostNames.ToList()));
        var connection = connectionFactory.CreateConnection();

        services.AddSingleton<IConnection>(connection);
        services.AddSingleton<IModel>(connection.CreateModel());

        return services;
    }
}
