using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Search.v1;

public class SearchCourtRentalsByHouseDayTimeCourtCommand : PaginationFilter, IRequest<PagedList<CourtRentalResponse>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Weekday { get; set; }
    public TimeSpan? StartTime { get; set; }
    public int? CourtFrom { get; set; }
    public int? CourtTo { get; set; }
}
