using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Payments.Domain.Events;

public sealed record PaymentUpdated : DomainEvent
{
    public required Payment Payment { get; init; }
}
