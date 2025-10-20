using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Create.v1;
public sealed record CreateCourtRentalCommand(
    [property: DefaultValue("Sample CourtRental")] string? Name,
    [property: DefaultValue(10)] decimal Price,
    [property: DefaultValue("Descriptive Description")] string? Description = null,
    [property: DefaultValue(null)] Guid? GroupId = null) : IRequest<CreateCourtRentalResponse>;
