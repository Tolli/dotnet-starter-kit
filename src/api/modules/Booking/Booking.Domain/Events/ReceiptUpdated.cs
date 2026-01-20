using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record ReceiptUpdated : DomainEvent
{
    public Receipt? Receipt { get; set; }
}
