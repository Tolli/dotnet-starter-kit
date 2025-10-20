using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Booking.Domain.Exceptions;
public sealed class GroupNotFoundException : NotFoundException
{
    public GroupNotFoundException(Guid id)
        : base($"group with id {id} not found")
    {
    }
}
