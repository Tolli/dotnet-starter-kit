using Ardalis.Specification;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;

public class GetCourtRentalSpecs : Specification<CourtRental, CourtRentalResponse>
{
    public GetCourtRentalSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Group);
    }
}
