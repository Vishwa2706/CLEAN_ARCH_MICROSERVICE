using Microsoft.EntityFrameworkCore;
using Notification.Application.Contracts;
using Notification.Domain.Models;
using Notification.Infrastruture.Repository;

namespace Notification.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly NotificationRepository _context;

    public NotificationService(NotificationRepository context)
    {
        _context = context;
    }

    public async Task<List<NotificationDto>> GetAllNotification(CancellationToken cancellationToken)
    {
        return await _context.Notification.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task CreateNotificationAsync(
        NotificationDto notification,
        CancellationToken cancellationToken
    )
    {
        _context.Notification.Add(notification);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
