using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record CourtRentalShareUpdated : DomainEvent
{
    public CourtRentalShare? CourtRentalShare { get; set; }
}
