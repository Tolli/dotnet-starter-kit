using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Booking.Domain.Exceptions;
public sealed class CourtRentalShareNotFoundException : NotFoundException
{
    public CourtRentalShareNotFoundException(Guid id)
        : base($"court rental share with id {id} not found")
    {
    }
}
