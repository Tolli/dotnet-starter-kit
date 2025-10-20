using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record CourtRentalCreated : DomainEvent
{
    public CourtRental? CourtRental { get; set; }
}
