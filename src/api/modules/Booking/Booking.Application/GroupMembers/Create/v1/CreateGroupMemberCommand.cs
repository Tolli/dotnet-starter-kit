using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Create.v1;
public sealed record CreateGroupMemberCommand(
    Guid GroupId,
    Guid CustomerId,
    [property: DefaultValue(10)] decimal Amount,
    [property: DefaultValue(10)] decimal Discount,
    [property: DefaultValue(false)] bool IsContact) : IRequest<CreateGroupMemberResponse>;
