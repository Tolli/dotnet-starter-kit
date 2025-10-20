using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Delete.v1;
public sealed record DeleteCourtRentalCommand(
    Guid Id) : IRequest;
