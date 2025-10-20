using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Delete.v1;
public sealed record DeleteGroupCommand(
    Guid Id) : IRequest;
