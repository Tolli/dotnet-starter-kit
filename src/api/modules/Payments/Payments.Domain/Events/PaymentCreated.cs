using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Payments.Domain.Events;

public sealed record PaymentCreated : DomainEvent
{
    public required Payment Payment { get; init; }
}
