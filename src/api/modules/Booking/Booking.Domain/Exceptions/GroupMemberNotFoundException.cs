using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Booking.Domain.Exceptions;
public sealed class GroupMemberNotFoundException : NotFoundException
{
    public GroupMemberNotFoundException(Guid id)
        : base($"groupmember with id {id} not found")
    {
    }
}
