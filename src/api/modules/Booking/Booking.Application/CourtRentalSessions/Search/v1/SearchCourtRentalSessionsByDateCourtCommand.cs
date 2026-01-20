using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Search.v1;

public class SearchCourtRentalSessionsByDateCourtCommand : PaginationFilter, IRequest<PagedList<CourtRentalSessionResponse>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? CourtFrom { get; set; }
    public int? CourtTo { get; set; }
}
