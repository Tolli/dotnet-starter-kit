using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record CourtRentalShareCreated : DomainEvent
{
    public CourtRentalShare? CourtRentalShare { get; set; }
}
