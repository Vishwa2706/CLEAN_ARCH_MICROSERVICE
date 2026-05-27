using Microsoft.EntityFrameworkCore;
using Notification.Domain.Models;

namespace Notification.Infrastruture.Repository;

public class NotificationRepository : DbContext
{
    public NotificationRepository(DbContextOptions<NotificationRepository> options)
        : base(options) { }

    public DbSet<NotificationDto> Notification => Set<NotificationDto>();
}
