namespace Notification.Domain.Models;

public class NotificationDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = default!;

    public string Message { get; set; } = default!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}
