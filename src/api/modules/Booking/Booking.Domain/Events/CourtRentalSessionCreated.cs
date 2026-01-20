using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record CourtRentalSessionCreated : DomainEvent
{
    public CourtRentalSession? CourtRentalSession { get; set; }
}
