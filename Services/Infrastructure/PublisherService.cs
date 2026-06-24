namespace main.Services.Infrastructure;

using RabbitMQ.Client;
using main.Services.Interfaces;
using System.Text;

public class PublisherService : IPublisherService {
    private readonly IConfiguration _configuration;
    
    public PublisherService(IConfiguration configuration) {
        _configuration = configuration;
    }

    public async Task PublishAsync(string queueName, string message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"]
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
            basicProperties: new RabbitMQ.Client.BasicProperties(),
            body: body
        );
    }
}