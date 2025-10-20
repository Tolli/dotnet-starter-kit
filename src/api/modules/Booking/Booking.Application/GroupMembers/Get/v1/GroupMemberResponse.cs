using FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
public sealed record GroupMemberResponse(Guid? Id, string Name, string? Description, decimal Price, GroupResponse? Group);
