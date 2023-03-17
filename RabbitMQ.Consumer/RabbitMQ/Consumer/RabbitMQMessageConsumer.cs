using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace RabbitMQ.Consumer.RabbitMQ.Consumer;

public class RabbitMQMessageConsumer : IMessageConsumer
{
    private readonly IModel channel;
    private readonly RabbitMqOptions options;

    public RabbitMQMessageConsumer(IConnection connection, IOptions<RabbitMqOptions> options)
    {
        this.channel = connection.CreateModel();
        this.options = options.Value;
    }

    public void Consume(string queueName, Action<byte[]> onMessageReceived)
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

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            onMessageReceived(body);
            await Task.Yield();
        };
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }
}