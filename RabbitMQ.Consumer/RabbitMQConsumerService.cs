using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Common;
using System.Text;

namespace RabbitMQ.Consumer;

public sealed class RabbitMQConsumerService : IHostedService
{
    private readonly ILogger<RabbitMQConsumerService> logger;
    private readonly RabbitMqConnection connection;
    private readonly RabbitMqSettings settings;
    private bool disposed;
    private IModel channel;

    public RabbitMQConsumerService(IOptions<RabbitMqSettings> options, ILogger<RabbitMqConnection> connectionLogger, ILogger<RabbitMQConsumerService> logger)
    {
        this.logger = logger;
        this.settings = options.Value;
        connection = new RabbitMqConnection(settings, connectionLogger);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!connection.IsConnected)
        {
            connection.TryConnect();
        }
        channel = connection.CreateModel();
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += ProcessMessage;

        channel.BasicConsume(queue: settings.Queue.Name, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    private void ProcessMessage(object model, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        try
        {
            logger.LogInformation("Received message: {Message}", message);

            channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing message: {Message}", message);

            channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
        }
    }

    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
            channel?.Dispose();
            connection?.Dispose();
        }
    }
}
