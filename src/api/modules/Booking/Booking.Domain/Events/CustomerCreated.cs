using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record CustomerCreated : DomainEvent
{
    public Customer? Customer { get; set; }
}
