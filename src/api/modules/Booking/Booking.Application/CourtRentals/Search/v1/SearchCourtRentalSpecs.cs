using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Search.v1;
public class SearchCourtRentalSpecs : EntitiesByPaginationFilterSpec<CourtRental, CourtRentalResponse>
{
    public SearchCourtRentalSpecs(SearchCourtRentalsCommand command)
        : base(command) =>
        Query
            .Include(p => p.Group)
            .Where(p => p.GroupId == command.GroupId!.Value, command.GroupId.HasValue)
            .Where(p => p.Weekday == Enum.Parse<DayOfWeek>(command.Weekday!), !string.IsNullOrEmpty(command.Weekday))
            .Where(p => p.StartTime >= command.FromTime!.Value, command.FromTime.HasValue)
            .Where(p => p.Court == command.Court!.Value, command.Court.HasValue)
            .Where(p => p.StartDate >= command.StartDate!.Value, command.StartDate.HasValue)
            .Where(p => p.EndDate <= command.EndDate!.Value, command.EndDate.HasValue)
            .OrderBy(p => p.Weekday).ThenBy(p => p.Court).ThenBy(p => p.StartTime);
}
