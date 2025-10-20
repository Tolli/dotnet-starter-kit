using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record GroupMemberCreated : DomainEvent
{
    public GroupMember? GroupMember { get; set; }
}
