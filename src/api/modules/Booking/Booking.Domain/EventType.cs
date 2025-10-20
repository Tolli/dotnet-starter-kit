using System.Text.RegularExpressions;
using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public class EventType : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = string.Empty;

    private EventType() { }

    private EventType(Guid id, string title)
    {
        Id = id;
        Title = title;
        QueueDomainEvent(new EventTypeCreated { EventType = this });
    }

    public static EventType Create(string title)
    {
        return new EventType(Guid.NewGuid(), title);
    }

    public EventType Update(string? title)
    {
        bool isUpdated = false;

        if (!string.IsNullOrWhiteSpace(title) && !string.Equals(Title, title, StringComparison.OrdinalIgnoreCase))
        {
            Title = title;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new EventTypeUpdated { EventType = this });
        }

        return this;
    }
}


