using Microsoft.Extensions.Logging;
using Polly.Retry;
using Polly;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace RabbitMQ.Common;

public class RabbitMqConnectionPool : IDisposable
{
    private readonly ConcurrentBag<IConnection> connections = new();
    private readonly ConnectionFactory connectionFactory;
    private readonly ILogger<RabbitMqConnectionPool> logger;
    private readonly RetryPolicy connectRetryPolicy;
    private bool disposed;

    private const int POOL_SIZE = 10;

    public RabbitMqConnectionPool(RabbitMqSettings settings, ILogger<RabbitMqConnectionPool> logger)
    {
        this.logger = logger;
        this.connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(CreateConnectionString(settings))
        };
        this.connectRetryPolicy = Policy
            .Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .Or<IOException>()
            .WaitAndRetry(settings.RetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan) =>
                {
                    logger.LogWarning(exception, "Failed to connect to RabbitMQ. Retrying in {Delay} seconds.", timeSpan.TotalSeconds);
                });

        InitializeConnections();
    }

    private void InitializeConnections()
    {
        for (int i = 0; i < POOL_SIZE; i++)
        {
            var connection = CreateConnection();
            if (connection.IsOpen)
            {
                connections.Add(connection);
            }
        }
    }

    public IConnection GetConnection()
    {
        IConnection connection = connections.FirstOrDefault(c => c.IsOpen);
        if (connection == null)
        {
            connection = CreateConnection();
            connections.Add(connection);
        }
        return connection;
    }

    private IConnection CreateConnection()
    {
        return connectRetryPolicy.Execute(() =>
        {
            var connection = connectionFactory.CreateConnection();
            connection.ConnectionShutdown += OnConnectionShutdown;
            return connection;
        });
    }

    private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        logger.LogWarning("Connection was shut down, reason: {ReplyText}", e.ReplyText);

        var connection = sender as IConnection;
        connections.TryTake(out connection);

        if (!disposed)
        {
            var newConnection = CreateConnection();
            if (newConnection.IsOpen)
            {
                connections.Add(newConnection);
            }
        }
    }

    private string CreateConnectionString(RabbitMqSettings settings)
    {
        void CheckNullOrInvalid(string value, string message)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(message, nameof(settings));
            }
        }

        CheckNullOrInvalid(settings.Username, "Username is required.");
        CheckNullOrInvalid(settings.Password, "Password is required.");
        CheckNullOrInvalid(settings.Host, "Host is required.");

        var connectionString = string.Format("amqp://{0}:{1}@{2}:{3}", settings.Username, settings.Password, settings.Host, settings.Port);
        return connectionString;
    }

    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
            foreach (var connection in connections)
            {
                connection.Dispose();
            }
        }
    }
}
