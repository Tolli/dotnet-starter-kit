using Ardalis.Specification;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;

public class GetCourtRentalSessionsByCourtRentalIdSpecs : EntitiesByPaginationFilterSpec<CourtRentalSession, CourtRentalSessionResponse>
{
    public GetCourtRentalSessionsByCourtRentalIdSpecs(GetCourtRentalSessionsByCourtRentalIdCommand command)
    : base(command) =>
        Query
            .Include(p => p.CourtRental)
            .Where(p => p.CourtRentalId == command.CourtRentalId);
}
