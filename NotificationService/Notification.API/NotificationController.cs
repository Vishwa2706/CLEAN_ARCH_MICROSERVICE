using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Query;

namespace Notification.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotificationsAsync(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetNotificationQuery(), cancellationToken);

        return Ok(result);
    }
}
