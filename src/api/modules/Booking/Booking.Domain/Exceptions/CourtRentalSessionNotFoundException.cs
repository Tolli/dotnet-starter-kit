using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Booking.Domain.Exceptions;
public sealed class CourtRentalSessionNotFoundException : NotFoundException
{
    public CourtRentalSessionNotFoundException(Guid id)
        : base($"court rental session with id {id} not found")
    {
    }
}
