using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Search.v1;

public class SearchGroupMembersCommand : PaginationFilter, IRequest<PagedList<GroupMemberResponse>>
{
    public Guid? GroupId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
}
