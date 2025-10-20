using FSH.Starter.WebApi.Booking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.Groups.EventHandlers;

public class GroupCreatedEventHandler(ILogger<GroupCreatedEventHandler> logger) : INotificationHandler<GroupCreated>
{
    public async Task Handle(GroupCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling group created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling group created domain event..");
    }
}
