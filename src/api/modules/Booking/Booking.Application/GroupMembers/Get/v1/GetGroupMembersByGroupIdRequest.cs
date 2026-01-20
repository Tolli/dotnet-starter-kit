using FSH.Framework.Core.Paging;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
public class GetGroupMembersByGroupIdCommand : PaginationFilter, IRequest<PagedList<GroupMemberResponse>>
{
    public Guid GroupId { get; set; }    
}
