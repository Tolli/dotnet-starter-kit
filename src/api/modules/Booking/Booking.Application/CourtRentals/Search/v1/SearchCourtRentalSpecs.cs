using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Search.v1;
public class SearchCourtRentalSpecs : EntitiesByPaginationFilterSpec<CourtRental, CourtRentalResponse>
{
    public SearchCourtRentalSpecs(SearchCourtRentalsCommand command)
        : base(command) =>
        Query
            .Include(p => p.Group)
            .OrderBy(c => c.Name, !command.HasOrderBy())
            .Where(p => p.GroupId == command.GroupId!.Value, command.GroupId.HasValue)
            .Where(p => p.Price >= command.MinimumRate!.Value, command.MinimumRate.HasValue)
            .Where(p => p.Price <= command.MaximumRate!.Value, command.MaximumRate.HasValue);
}
