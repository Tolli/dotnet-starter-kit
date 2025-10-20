using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Booking.Domain.Exceptions;
public sealed class EventTypeNotFoundException : NotFoundException
{
    public EventTypeNotFoundException(Guid id)
        : base($"event type with id {id} not found")
    {
    }
}
