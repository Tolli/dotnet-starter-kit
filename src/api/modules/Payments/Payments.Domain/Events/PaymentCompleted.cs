using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Payments.Domain.Events;

public sealed record PaymentCompleted : DomainEvent
{
    public required Payment Payment { get; init; }
}
