using MediatR;
using Notification.Application.Contracts;
using Notification.Domain.Models;

namespace Notification.Application.Query;

public class GetNotificationQuery : IRequest<List<NotificationDto>> { };

public class GetNotificationQueryHandler
    : IRequestHandler<GetNotificationQuery, List<NotificationDto>>
{
    private readonly INotificationService _notificationService;

    public GetNotificationQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<List<NotificationDto>> Handle(
        GetNotificationQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _notificationService.GetAllNotification(cancellationToken);
    }
}
