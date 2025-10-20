using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record GroupMemberUpdated : DomainEvent
{
    public GroupMember? GroupMember { get; set; }
}
