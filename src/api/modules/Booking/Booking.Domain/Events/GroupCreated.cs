using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record GroupCreated : DomainEvent
{
    public Group? Group { get; set; }
}
