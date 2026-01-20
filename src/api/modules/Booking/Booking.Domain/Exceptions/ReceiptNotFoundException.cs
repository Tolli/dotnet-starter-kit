using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Booking.Domain.Exceptions;
public sealed class ReceiptNotFoundException : NotFoundException
{
    public ReceiptNotFoundException(Guid id)
        : base($"receipt with id {id} not found")
    {
    }
}
