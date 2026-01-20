using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Search.v1;
public class SearchCourtRentalSessionsByDateCourtSpecs : EntitiesByPaginationFilterSpec<CourtRentalSession, CourtRentalSessionResponse>
{
    public SearchCourtRentalSessionsByDateCourtSpecs(SearchCourtRentalSessionsByDateCourtCommand command)
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
            .Include(p => p.CourtRental)
            .Where(p => p.Court >= _courtStart && p.Court <= _courtEnd)
            .Where(p => p.StartDate >= command.StartDate, command.StartDate.HasValue)
            .Where(p => p.EndDate <= command.EndDate, command.EndDate.HasValue)
            .OrderBy(p => p.Court).ThenBy(p => p.StartDate);
    }
}
