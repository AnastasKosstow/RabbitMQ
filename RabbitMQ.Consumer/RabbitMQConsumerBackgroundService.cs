using Microsoft.Extensions.Hosting;
using RabbitMQ.Consumer.Dispatcher;
using RabbitMQ.Consumer.Events;
using RabbitMQ.Consumer.RabbitMQ.Consumer;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Consumer;

public sealed class RabbitMQConsumerBackgroundService : BackgroundService
{
    private readonly IMessageConsumer _messageConsumer;
    private readonly IEventDispatcher _eventDispatcher;

    public RabbitMQConsumerBackgroundService(IMessageConsumer messageConsumer, IEventDispatcher eventDispatcher)
    {
        _messageConsumer = messageConsumer;
        _eventDispatcher = eventDispatcher;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageConsumer.Consume("rabbit", payload =>
        {
            var message = Encoding.UTF8.GetString(payload);
            var @event = JsonSerializer.Deserialize<RabbitMqEvent>(message);
            _eventDispatcher.DispatchAsync(@event, stoppingToken);
        });

        return Task.CompletedTask;
    }
}
