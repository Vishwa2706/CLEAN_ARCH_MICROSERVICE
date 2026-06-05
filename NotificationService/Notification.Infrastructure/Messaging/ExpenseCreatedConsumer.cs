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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { Uri = new Uri(_settings.ConnectionString) };

        var connection = await factory.CreateConnectionAsync(stoppingToken);

        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: "expense-created",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();

            var json = Encoding.UTF8.GetString(body);

            var expenseEvent = JsonSerializer.Deserialize<ExpenseCreatedEvent>(json);

            if (expenseEvent != null)
            {
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
            }

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false, stoppingToken);
        };

        await channel.BasicConsumeAsync(
            queue: "expense-created",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
