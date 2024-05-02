using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Common;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Producer.Services;

public class RabbitMqPublisherWithConnection : IRabbitMqPublisherWithConnection, IDisposable
{
    private readonly RabbitMqSettings settings;
    private readonly RabbitMqConnection connection;
    private readonly RabbitMqConnectionPool connectionPool;
    private bool disposed;

    public RabbitMqPublisherWithConnection(IOptions<RabbitMqSettings> options, ILogger<RabbitMqConnection> connectionLogger, ILogger<RabbitMqConnectionPool> connectionPoolLogger)
    {
        settings = options.Value;
        connection = new RabbitMqConnection(settings, connectionLogger);
        connectionPool = new RabbitMqConnectionPool(settings, connectionPoolLogger);
    }

    public void Publish<T>(T command, string routingKey)
    {
        if (!connection.IsConnected)
        {
            connection.TryConnect();
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
            connection?.Dispose();
        }
    }
}
