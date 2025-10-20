using Ardalis.Specification;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;

public class GetGroupMemberSpecs : Specification<GroupMember, GroupMemberResponse>
{
    public GetGroupMemberSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id);
    }
}
