using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Delete.v1;
public sealed record DeleteGroupMemberCommand(
    Guid Id) : IRequest;
