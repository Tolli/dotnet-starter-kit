using FSH.Starter.WebApi.Payments.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Payments.Application.Payments.EventHandlers;

public sealed class PaymentCreatedEventHandler(ILogger<PaymentCreatedEventHandler> logger)
    : INotificationHandler<PaymentCreated>
{
    public Task Handle(PaymentCreated notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("handling payment created domain event for payment {PaymentId}..", notification.Payment.Id);
        // Add additional logic here (send notifications, update other aggregates, etc.)
        return Task.CompletedTask;
    }
}
