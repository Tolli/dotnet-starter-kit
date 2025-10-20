using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public class CourtRental : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string Time { get; private set; }
    public string Duration { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }


    public Guid? GroupId { get; private set; }
    public virtual Group Group { get; private set; } = default!;

    private CourtRental() { }

    private CourtRental(Guid id, string name, string? description, decimal price, Guid? groupId)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        GroupId = groupId;

        QueueDomainEvent(new CourtRentalCreated { CourtRental = this });
    }

    public static CourtRental Create(string name, string? description, decimal price, Guid? groupId)
    {
        return new CourtRental(Guid.NewGuid(), name, description, price, groupId);
    }

    public CourtRental Update(string? name, string? description, decimal? price, Guid? brandId)
    {
        bool isUpdated = false;

        if (!string.IsNullOrWhiteSpace(name) && !string.Equals(Name, name, StringComparison.OrdinalIgnoreCase))
        {
            Name = name;
            isUpdated = true;
        }

        if (!string.Equals(Description, description, StringComparison.OrdinalIgnoreCase))
        {
            Description = description;
            isUpdated = true;
        }

        if (price.HasValue && Price != price.Value)
        {
            Price = price.Value;
            isUpdated = true;
        }

        if (brandId.HasValue && brandId.Value != Guid.Empty && GroupId != brandId.Value)
        {
            GroupId = brandId.Value;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new CourtRentalUpdated { CourtRental = this });
        }

        return this;
    }
}

