using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Search.v1;

public class SearchCourtRentalsCommand : PaginationFilter, IRequest<PagedList<CourtRentalResponse>>
{
    public Guid? GroupId { get; set; }
    public string? Weekday { get; set; }
    public TimeSpan? FromTime { get; set; }
    public int? Court { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
