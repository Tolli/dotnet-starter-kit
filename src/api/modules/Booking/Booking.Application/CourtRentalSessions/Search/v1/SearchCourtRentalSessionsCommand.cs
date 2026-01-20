using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Search.v1;

public class SearchCourtRentalSessionsCommand : PaginationFilter, IRequest<PagedList<CourtRentalSessionResponse>>
{
    public Guid? CourtRentalId { get; set; }
}
