using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Create.v1;
public sealed record CreateCourtRentalCommand(
    [property: DefaultValue("2025-09-30")] DateTime StartDate,
    [property: DefaultValue("10:00:00")] TimeSpan StartTime,
    [property: DefaultValue("2026-05-31")] DateTime EndDate,
    [property: DefaultValue("Monday")] string Weekday,
    [property: DefaultValue(10)] decimal Amount,
    [property: DefaultValue(0)] decimal Discount,
    [property: DefaultValue(0)] int Duration,
    [property: DefaultValue(1)] int Court,
    [property: DefaultValue(null)] Guid? GroupId = null) : IRequest<CreateCourtRentalResponse>;
