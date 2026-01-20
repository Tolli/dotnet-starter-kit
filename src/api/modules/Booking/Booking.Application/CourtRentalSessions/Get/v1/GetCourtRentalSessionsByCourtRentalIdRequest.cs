using FSH.Framework.Core.Paging;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
public class GetCourtRentalSessionsByCourtRentalIdCommand : PaginationFilter, IRequest<PagedList<CourtRentalSessionResponse>>
{
    public Guid CourtRentalId { get; set; }    
}
