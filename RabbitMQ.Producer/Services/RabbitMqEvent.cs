namespace RabbitMQ.Producer.Events;

public class RabbitMqEvent
{
    public Guid Id { get; }

    public RabbitMqEvent(Guid id)
        => Id = id;
}
