using FSH.Starter.WebApi.Booking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.EventHandlers;

public class GroupMemberCreatedEventHandler(ILogger<GroupMemberCreatedEventHandler> logger) : INotificationHandler<GroupMemberCreated>
{
    public async Task Handle(GroupMemberCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling groupmember created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling groupmember created domain event..");
    }
}

