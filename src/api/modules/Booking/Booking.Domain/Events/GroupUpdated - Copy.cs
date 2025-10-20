using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record GroupUpdated : DomainEvent
{
    public Group? Group { get; set; }
}
