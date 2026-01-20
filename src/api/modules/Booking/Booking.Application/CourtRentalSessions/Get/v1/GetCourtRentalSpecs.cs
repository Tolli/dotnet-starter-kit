using Ardalis.Specification;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;

public class GetCourtRentalSessionSpecs : Specification<CourtRentalSession, CourtRentalSessionResponse>
{
    public GetCourtRentalSessionSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            .Include(p => p.CourtRental);
    }
}
