using System.Text;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Update.v1;
public sealed record UpdateGroupMemberCommand(
    Guid Id,
    Guid GroupId,
    Guid CustomerId,
    decimal Amount,
    decimal Discount,
    bool IsContact) : IRequest<UpdateGroupMemberResponse>;
