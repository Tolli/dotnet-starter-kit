using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
public class GetGroupMemberRequest : IRequest<GroupMemberResponse>
{
    public Guid Id { get; set; }
    public GetGroupMemberRequest(Guid id) => Id = id;
}
