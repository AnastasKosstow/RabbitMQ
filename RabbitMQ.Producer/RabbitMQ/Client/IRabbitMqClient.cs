namespace RabbitMQ.Producer.RabbitMQ.Client;

public interface IRabbitMqClient
{
    void Send(object message);
}
