using System.Text;
using System.Text.Json;
using Expense.Application.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.Common.Events;

namespace Expense.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;
    }

    public async Task PublishExpenseCreatedAsync(
        ExpenseCreatedEvent message,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine($"RabbitMQ URI = {_settings.ConnectionString}");
        var factory = new ConnectionFactory { Uri = new Uri(_settings.ConnectionString) };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);

        await using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            queue: "expense-created",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken
        );

        var payload = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(payload);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "expense-created",
            mandatory: false,
            body: body,
            cancellationToken: cancellationToken
        );
    }
}
