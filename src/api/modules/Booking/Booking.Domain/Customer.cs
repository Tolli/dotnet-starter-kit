using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public class Customer : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public virtual ICollection<GroupMember> Groups { get; set; } = new HashSet<GroupMember>();

    private Customer() { }

    private Customer(Guid id, string name, string? description, decimal price)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;

        QueueDomainEvent(new CustomerCreated { Customer = this });
    }

    public static Customer Create(string name, string? description, decimal price, Guid? brandId)
    {
        return new Customer(Guid.NewGuid(), name, description, price);
    }

    public Customer Update(string? name, string? description, decimal? price)
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

        if (isUpdated)
        {
            QueueDomainEvent(new CustomerUpdated { Customer = this });
        }

        return this;
    }
}

