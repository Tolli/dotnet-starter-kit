using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Search.v1;

public class SearchGroupsCommand : PaginationFilter, IRequest<PagedList<GroupResponse>>
{
    public string? Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? ContactId { get; set; }
}
