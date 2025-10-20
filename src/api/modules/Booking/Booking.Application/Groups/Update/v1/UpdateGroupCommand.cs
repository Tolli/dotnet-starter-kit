using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Update.v1;
public sealed record UpdateGroupCommand(
    Guid Id,
    string? Name,
    DateTime StartDate,
    DateTime EndDate,
    Guid? ContactId) : IRequest<UpdateGroupResponse>;
