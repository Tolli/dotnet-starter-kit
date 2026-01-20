using FSH.Starter.WebApi.Booking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.EventHandlers;

public class CourtRentalSessionCreatedEventHandler(ILogger<CourtRentalSessionCreatedEventHandler> logger) : INotificationHandler<CourtRentalSessionCreated>
{
    public async Task Handle(CourtRentalSessionCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling courtrentalsession created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling courtrentalsession created domain event..");
    }
}

