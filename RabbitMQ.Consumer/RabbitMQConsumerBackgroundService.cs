using Microsoft.Extensions.Hosting;
using RabbitMQ.Consumer.Dispatcher;
using RabbitMQ.Consumer.Events;
using RabbitMQ.Consumer.RabbitMQ.Consumer;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Consumer;

public sealed class RabbitMQConsumerBackgroundService : BackgroundService
{
    private readonly IMessageConsumer messageConsumer;
    private readonly IEventDispatcher eventDispatcher;

    public RabbitMQConsumerBackgroundService(IMessageConsumer messageConsumer, IEventDispatcher eventDispatcher)
    {
        this.messageConsumer = messageConsumer;
        this.eventDispatcher = eventDispatcher;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        messageConsumer.Consume("rabbit", payload =>
        {
            var message = Encoding.UTF8.GetString(payload);
            var @event = JsonSerializer.Deserialize<RabbitMqEvent>(message);
            eventDispatcher.DispatchAsync(@event, cancellationToken);
        });

        return Task.CompletedTask;
    }
}
