using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;
public class GetGroupRequest : IRequest<GroupResponse>
{
    public Guid Id { get; set; }
    public GetGroupRequest(Guid id) => Id = id;
}
