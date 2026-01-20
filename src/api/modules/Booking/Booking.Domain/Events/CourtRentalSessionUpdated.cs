using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record CourtRentalSessionUpdated : DomainEvent
{
    public CourtRentalSession? CourtRentalSession { get; set; }
}
