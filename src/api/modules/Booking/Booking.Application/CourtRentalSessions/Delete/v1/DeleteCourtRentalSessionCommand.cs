using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Delete.v1;
public sealed record DeleteCourtRentalSessionCommand(
    Guid Id) : IRequest;
