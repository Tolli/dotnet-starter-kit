using FSH.Starter.WebApi.Booking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.EventHandlers;

public class CourtRentalCreatedEventHandler(ILogger<CourtRentalCreatedEventHandler> logger) : INotificationHandler<CourtRentalCreated>
{
    public async Task Handle(CourtRentalCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling courtrental created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling courtrental created domain event..");
    }
}

