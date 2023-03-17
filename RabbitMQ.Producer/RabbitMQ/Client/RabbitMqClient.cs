using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitMQ.Producer.RabbitMQ.Client;

internal sealed class RabbitMqClient : IRabbitMqClient
{
    private readonly RabbitMqOptions options;
    private readonly IModel channel;

    public RabbitMqClient(IModel channel, IOptions<RabbitMqOptions> options)
    {
        this.channel = channel;
        this.options = options.Value;
    }

    public void Send(object message)
    {
        channel.ExchangeDeclare(
            exchange: options.Exchange.Name,
            type: options.Exchange.Type,
            durable: options.Exchange.Durable,
            autoDelete: options.Exchange.AutoDelete,
            arguments: null);

        channel.QueueDeclare(
            queue: options.Queue.Name,
            durable: options.Queue.Durable,
            exclusive: options.Queue.Exclusive,
            autoDelete: options.Queue.AutoDelete,
            arguments: null);

        channel.QueueBind(queue: options.Queue.Name, exchange: options.Exchange.Name, routingKey: "rabbit_key");

        string json = JsonSerializer.Serialize(message);
        ReadOnlySpan<byte> body = Encoding.UTF8.GetBytes(json);

        channel.ConfirmSelect();
        channel.BasicPublish(options.Exchange.Name, "rabbit_key", null, body.ToArray());
        channel.WaitForConfirmsOrDie();
    }
}
