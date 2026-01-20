using Ardalis.Specification;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;

public class GetGroupMembersByGroupIdSpecs : EntitiesByPaginationFilterSpec<GroupMember, GroupMemberResponse>
{
    public GetGroupMembersByGroupIdSpecs(GetGroupMembersByGroupIdCommand command)
    : base(command) =>
        Query
            .Where(p => p.GroupId == command.GroupId)
            .OrderBy(c => c.Customer.Name);
}
