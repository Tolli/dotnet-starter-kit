using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Booking.Domain.Exceptions;
public sealed class CustomerNotFoundException : NotFoundException
{
    public CustomerNotFoundException(Guid id)
        : base($"customer with id {id} not found")
    {
    }
}
