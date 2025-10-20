using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public class GroupMember : AuditableEntity, IAggregateRoot
{
    public decimal Price { get; private set; }

    public decimal Percentage { get; private set; }

    public bool IsContact { get; private set; }
    
    public Guid CustomerId { get; private set; }
    public virtual Customer Customer { get; private set; } = default!;

    public Guid GroupId { get; private set; }
    public virtual Group Group { get; private set; } = default!;
    private GroupMember(Guid id, Guid groupId, Guid customerId, decimal price, decimal percentage, bool isContact)
    {
        Id = id;
        GroupId = groupId;
        CustomerId = customerId;
        Price = price;
        Percentage = percentage;
        IsContact = isContact;

        QueueDomainEvent(new GroupMemberCreated { GroupMember = this });
    }

    public static GroupMember Create(Guid groupId, Guid customerId, decimal price, decimal percentage, bool isContact)
    {
        return new GroupMember(Guid.NewGuid(), groupId, customerId, price, percentage, isContact);
    }

    public GroupMember Update(Guid? groupId, Guid? customerId, decimal price, decimal percentage, bool isContact)
    {
        bool isUpdated = false;

        if (Price != price)
        {
            Price = price;
            isUpdated = true;
        }

        if (Percentage != percentage)
        {
            Percentage = percentage;
            isUpdated = true;
        }

        if(IsContact != isContact)
        {
            IsContact = isContact;
            isUpdated = true;
        }

        if (groupId.HasValue && groupId.Value != Guid.Empty && GroupId != groupId.Value)
        {
            GroupId = groupId.Value;
            isUpdated = true;
        }

        if (customerId.HasValue && customerId.Value != Guid.Empty && CustomerId != customerId.Value)
        {
            CustomerId = customerId.Value;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new GroupMemberUpdated { GroupMember = this });
        }

        return this;
    }
}

