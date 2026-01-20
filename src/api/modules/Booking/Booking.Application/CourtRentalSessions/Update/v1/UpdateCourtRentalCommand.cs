using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Update.v1;
public sealed record UpdateCourtRentalSessionCommand(
    Guid Id,
    DateTime? StartDate,
    DateTime? EndDate,
    int Court,
    Guid? CourtRentalId = null) : IRequest<UpdateCourtRentalSessionResponse>;
