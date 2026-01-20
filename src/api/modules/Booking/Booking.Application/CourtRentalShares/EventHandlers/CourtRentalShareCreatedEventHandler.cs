using FSH.Starter.WebApi.Booking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalShares.EventHandlers;

public class CourtRentalShareCreatedEventHandler(ILogger<CourtRentalShareCreatedEventHandler> logger) : INotificationHandler<CourtRentalShareCreated>
{
    public async Task Handle(CourtRentalShareCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling courtrentalshare created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling courtrentalshare created domain event..");
    }
}

