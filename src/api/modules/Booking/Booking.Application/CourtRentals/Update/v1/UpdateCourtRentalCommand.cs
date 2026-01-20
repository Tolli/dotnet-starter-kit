using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Update.v1;
public sealed record UpdateCourtRentalCommand(
    Guid Id,
    DateTime? StartDate,
    TimeSpan? StartTime,
    DateTime? EndDate,
    string? Weekday,
    decimal Amount,
    decimal Discount,
    int? Duration,
    int Court,
    Guid? GroupId = null) : IRequest<UpdateCourtRentalResponse>;
