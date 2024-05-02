namespace RabbitMQ.Producer.Services;

public interface IRabbitMqPublisherWithConnectionPool
{
    void Publish<T>(T command, string routingKey);
}
