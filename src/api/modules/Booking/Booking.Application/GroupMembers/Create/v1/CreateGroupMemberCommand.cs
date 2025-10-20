using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Create.v1;
public sealed record CreateGroupMemberCommand(
    Guid GroupId,
    Guid CustomerId,
    [property: DefaultValue(10)] decimal Price,
    [property: DefaultValue(10)] decimal Percentage,
    [property: DefaultValue(false)] bool IsContact) : IRequest<CreateGroupMemberResponse>;
