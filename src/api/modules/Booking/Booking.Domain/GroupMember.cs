using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public class GroupMember : AuditableEntity, IAggregateRoot
{
    public decimal Amount { get; private set; }

    public decimal Discount { get; private set; }

    public bool IsContact { get; private set; }
    
    public Guid CustomerId { get; private set; }
    public virtual Customer Customer { get; private set; } = default!;

    public Guid GroupId { get; private set; }
    public virtual Group Group { get; private set; } = default!;
    public virtual ICollection<Receipt>? Receipts { get; private set; } = new HashSet<Receipt>();
    private GroupMember(Guid id, Guid groupId, Guid customerId, decimal amount, decimal discount, bool isContact)
    {
        Id = id;
        GroupId = groupId;
        CustomerId = customerId;
        Amount = amount;
        Discount = discount;
        IsContact = isContact;

        QueueDomainEvent(new GroupMemberCreated { GroupMember = this });
    }

    public static GroupMember Create(Guid groupId, Guid customerId, decimal amount, decimal discount, bool isContact)
    {
        return new GroupMember(Guid.NewGuid(), groupId, customerId, amount, discount, isContact);
    }

    public GroupMember Update(Guid? groupId, Guid? customerId, decimal amount, decimal discount, bool isContact)
    {
        bool isUpdated = false;

        if (Amount != amount)
        {
            Amount = amount;
            isUpdated = true;
        }

        if (Discount != discount)
        {
            Discount = discount;
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

