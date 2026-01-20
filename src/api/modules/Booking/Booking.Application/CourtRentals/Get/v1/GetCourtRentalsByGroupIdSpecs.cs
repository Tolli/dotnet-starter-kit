using Ardalis.Specification;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;

public class GetCourtRentalsByGroupIdSpecs : EntitiesByPaginationFilterSpec<CourtRental, CourtRentalResponse>
{
    public GetCourtRentalsByGroupIdSpecs(GetCourtRentalsByGroupIdCommand command)
    : base(command) =>
        Query
            .Include(p => p.CourtRentalShares)
            .Where(p => p.GroupId == command.GroupId);
}
