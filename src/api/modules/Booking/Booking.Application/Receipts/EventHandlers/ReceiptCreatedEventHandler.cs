using FSH.Starter.WebApi.Booking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.EventHandlers;

public class ReceiptCreatedEventHandler(ILogger<ReceiptCreatedEventHandler> logger) : INotificationHandler<ReceiptCreated>
{
    public async Task Handle(ReceiptCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling receipt created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling receipt created domain event..");
    }
}

