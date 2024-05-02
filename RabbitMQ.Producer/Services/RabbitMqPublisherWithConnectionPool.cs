using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Common;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Producer.Services;

public class RabbitMqPublisherWithConnectionPool : IRabbitMqPublisherWithConnectionPool, IDisposable
{
    private readonly RabbitMqSettings settings;
    private readonly RabbitMqConnectionPool connectionPool;
    private bool disposed;

    public RabbitMqPublisherWithConnectionPool(IOptions<RabbitMqSettings> options, ILogger<RabbitMqConnectionPool> connectionPoolLogger)
    {
        settings = options.Value;
        connectionPool = new RabbitMqConnectionPool(settings, connectionPoolLogger);
    }

    public void Publish<T>(T command, string routingKey)
    {
        var connection = connectionPool.GetConnection();
        if (connection == null || !connection.IsOpen)
        {
            return;
        }

        using var channel = connection.CreateModel();
        DeclareAndBind(channel, routingKey);

        var body = SerializeCommand(command);

        channel.ConfirmSelect();
        channel.BasicPublish(
            exchange: settings.Exchange.Name,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: null,
            body: body);

        channel.WaitForConfirmsOrDie();
    }

    private void DeclareAndBind(IModel channel, string routingKey)
    {
        channel.ExchangeDeclare(
            exchange: settings.Exchange.Name,
            type: settings.Exchange.Type,
            durable: settings.Exchange.Durable,
            autoDelete: settings.Exchange.AutoDelete,
            arguments: null);

        channel.QueueDeclare(
            queue: settings.Queue.Name,
            durable: settings.Queue.Durable,
            exclusive: settings.Queue.Exclusive,
            autoDelete: settings.Queue.AutoDelete,
            arguments: null);

        channel.QueueBind(
            queue: settings.Queue.Name,
            exchange: settings.Exchange.Name,
            routingKey: routingKey);
    }

    private byte[] SerializeCommand<TCommand>(TCommand command)
    {
        string json = JsonSerializer.Serialize(command);
        return Encoding.UTF8.GetBytes(json);
    }

    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
            connectionPool.Dispose();
        }
    }
}
