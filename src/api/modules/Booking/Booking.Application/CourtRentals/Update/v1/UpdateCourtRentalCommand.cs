using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Update.v1;
public sealed record UpdateCourtRentalCommand(
    Guid Id,
    string? Name,
    decimal Price,
    string? Description = null,
    Guid? GroupId = null) : IRequest<UpdateCourtRentalResponse>;
