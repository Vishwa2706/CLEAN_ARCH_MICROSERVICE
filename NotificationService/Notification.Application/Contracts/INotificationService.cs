using Notification.Domain.Models;

namespace Notification.Application.Contracts;

public interface INotificationService
{
    Task<List<NotificationDto>> GetAllNotification(CancellationToken cancellationToken);
}
