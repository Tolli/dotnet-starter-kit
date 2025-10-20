using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record EventTypeCreated : DomainEvent
{
    public EventType? EventType { get; set; }
}
