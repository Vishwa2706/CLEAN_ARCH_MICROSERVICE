using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Notification.Application.Contracts;
using Notification.Domain.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Common.Events;

namespace Notification.Infrastructure.Messaging;

public class ExpenseCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqSettings _settings;

    public ExpenseCreatedConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqSettings> options
    )
    {
        _scopeFactory = scopeFactory;
        _settings = options.Value;
    }

    private async Task PublishToDlqAsync(
        IChannel channel,
        byte[] body,
        CancellationToken cancellationToken
    )
    {
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "expense-created-dlq",
            body: body,
            cancellationToken: cancellationToken
        );
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { Uri = new Uri(_settings.ConnectionString) };

        var connection = await factory.CreateConnectionAsync(stoppingToken);

        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // Main Queue
        await channel.QueueDeclareAsync(
            queue: "expense-created",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        // Dead Letter Queue
        await channel.QueueDeclareAsync(
            queue: "expense-created-dlq",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            const int maxRetries = 3;

            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var body = eventArgs.Body.ToArray();

                    var json = Encoding.UTF8.GetString(body);

                    var expenseEvent = JsonSerializer.Deserialize<ExpenseCreatedEvent>(json);

                    if (expenseEvent == null)
                    {
                        throw new Exception("Failed to deserialize ExpenseCreatedEvent");
                    }

                    using var scope = _scopeFactory.CreateScope();

                    var notificationService =
                        scope.ServiceProvider.GetRequiredService<INotificationService>();

                    await notificationService.CreateNotificationAsync(
                        new NotificationDto
                        {
                            UserId = expenseEvent.UserId,
                            Title = "Expense Created",
                            Message =
                                $"Expense ₹{expenseEvent.Amount} created in {expenseEvent.Category}",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow,
                        },
                        stoppingToken
                    );

                    Console.WriteLine($"Message processed successfully on attempt {attempt}");

                    await channel.BasicAckAsync(eventArgs.DeliveryTag, false, stoppingToken);

                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {attempt} failed. Error: {ex.Message}");

                    if (attempt == maxRetries)
                    {
                        Console.WriteLine("Maximum retries reached. Moving message to DLQ.");

                        await PublishToDlqAsync(channel, eventArgs.Body.ToArray(), stoppingToken);

                        Console.WriteLine("Message successfully moved to expense-created-dlq");

                        await channel.BasicAckAsync(eventArgs.DeliveryTag, false, stoppingToken);

                        return;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        };

        await channel.BasicConsumeAsync(
            queue: "expense-created",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        Console.WriteLine("ExpenseCreatedConsumer started and listening on queue: expense-created");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
