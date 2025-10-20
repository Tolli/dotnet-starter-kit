using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record CustomerUpdated : DomainEvent
{
    public Customer? Customer { get; set; }
}
