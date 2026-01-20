using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain.Events;
public sealed record ReceiptCreated : DomainEvent
{
    public Receipt? Receipt { get; set; }
}
