using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Search.v1;
public class SearchCourtRentalByHouseDayTimeCourtSpecs : EntitiesByPaginationFilterSpec<CourtRental, CourtRentalResponse>
{
    public SearchCourtRentalByHouseDayTimeCourtSpecs(SearchCourtRentalsByHouseDayTimeCourtCommand command)
        : base(command)
    {
        int _courtStart = command.CourtFrom.HasValue ? command.CourtFrom.Value : 1;
        int _courtEnd = command.CourtTo.HasValue ? command.CourtTo.Value : (_courtStart + 7);
        //if (command.House.HasValue && !command.Court.HasValue)
        //{
        //    switch (command.House.Value)
        //    {
        //        case 1:
        //            _courtStart = 1;
        //            _courtEnd = 5;
        //            break;
        //        case 2:
        //            _courtStart = 6;
        //            _courtEnd = 17;
        //            break;
        //        default:
        //            break;
        //    }
        //}
        Query
            .Include(p => p.CourtRentalShares)
            .Where(p => p.Weekday == Enum.Parse<DayOfWeek>(command.Weekday, true), !string.IsNullOrEmpty(command.Weekday))
            .Where(p => p.Court >= _courtStart && p.Court <= _courtEnd)
            .Where(p => p.StartTime >= command.StartTime, command.StartTime.HasValue)
            .Where(p => p.StartDate >= command.StartDate, command.StartDate.HasValue)
            .Where(p => p.EndDate <= command.EndDate, command.EndDate.HasValue)
            .OrderBy(p => p.Court).ThenBy(p => p.StartDate).ThenBy(p => p.StartTime);
    }
}
