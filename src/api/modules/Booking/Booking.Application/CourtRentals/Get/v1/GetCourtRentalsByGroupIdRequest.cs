using FSH.Framework.Core.Paging;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
public class GetCourtRentalsByGroupIdCommand : PaginationFilter, IRequest<PagedList<CourtRentalResponse>>
{
    public Guid GroupId { get; set; }    
}
