using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Create.v1;
public sealed record CreateCourtRentalSessionCommand(
    [property: DefaultValue("")] DateTime StartDate,
    [property: DefaultValue("")] DateTime EndDate,
    [property: DefaultValue(1)] int Court,
    [property: DefaultValue(null)] Guid? CourtRentalId = null) : IRequest<CreateCourtRentalSessionResponse>;
