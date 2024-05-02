namespace RabbitMQ.Producer.Services;

public interface IRabbitMqPublisherWithConnection
{
    void Publish<T>(T command, string routingKey);
}
