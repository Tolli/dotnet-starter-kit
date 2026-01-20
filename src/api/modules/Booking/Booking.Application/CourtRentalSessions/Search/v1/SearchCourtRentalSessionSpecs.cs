using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Search.v1;
public class SearchCourtRentalSessionSpecs : EntitiesByPaginationFilterSpec<CourtRentalSession, CourtRentalSessionResponse>
{
    public SearchCourtRentalSessionSpecs(SearchCourtRentalSessionsCommand command)
        : base(command) =>
        Query
            .Include(p => p.CourtRental)
            .Where(p => p.CourtRentalId == command.CourtRentalId!.Value, command.CourtRentalId.HasValue);
}
