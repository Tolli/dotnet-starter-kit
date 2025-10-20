using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Booking.Domain.Exceptions;
public sealed class CourtRentalNotFoundException : NotFoundException
{
    public CourtRentalNotFoundException(Guid id)
        : base($"court rental with id {id} not found")
    {
    }
}
