using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public class Receipt : AuditableEntity, IAggregateRoot
{
    public decimal AmountTotal { get; private set; }
    public decimal AmountPaid { get; private set; }
    public decimal Discount { get; private set; }
    public string? ExternalReference { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime ReceiptDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public Guid GroupMemberId { get; private set; }
    public virtual GroupMember GroupMember { get; private set; } = default!;
    private Receipt(Guid id, Guid groupMemberId, decimal amountTotal, decimal amountPaid, decimal discount, DateTime receiptDate, DateTime dueDate, string? externalReference)
    {
        Id = id;
        GroupMemberId = groupMemberId;
        AmountTotal = amountTotal;
        AmountPaid = amountPaid;
        Discount = discount;
        ReceiptDate = receiptDate;
        DueDate = dueDate;
        IsPaid = amountPaid >= amountTotal;
        ExternalReference = externalReference;

        QueueDomainEvent(new ReceiptCreated { Receipt = this });
    }

    public static Receipt Create(Guid groupMemberId, decimal amountTotal, decimal amountPaid, decimal discount, DateTime receiptDate, DateTime dueDate, string? externalReference)
    {
        return new Receipt(Guid.NewGuid(), groupMemberId, amountTotal, amountPaid, discount, receiptDate, dueDate, externalReference);
    }

    public Receipt Update(Guid? groupMemberId, decimal? amountTotal, decimal? amountPaid, decimal? discount, DateTime? receiptDate, DateTime? dueDate, string? externalReference)
    {
        bool isUpdated = false;

        if (amountTotal.HasValue && AmountTotal != amountTotal.Value)
        {
            AmountTotal = amountTotal.Value;
            IsPaid = amountPaid >= amountTotal;
            isUpdated = true;
        }

        if (amountPaid.HasValue && AmountPaid != amountPaid.Value)
        {
            AmountPaid = amountPaid.Value;
            IsPaid = amountPaid >= amountTotal;
            isUpdated = true;
        }        

        if (discount.HasValue && Discount != discount.Value)
        {
            Discount = discount.Value;
            isUpdated = true;
        }

        if (groupMemberId.HasValue && GroupMemberId != groupMemberId)
        {
            GroupMemberId = groupMemberId.Value;
            isUpdated = true;
        }
        
        if (receiptDate.HasValue && ReceiptDate != receiptDate)
        {
            ReceiptDate = receiptDate.Value;
            isUpdated = true;
        }

        if (dueDate.HasValue && DueDate != dueDate)
        {
            DueDate = dueDate.Value;
            isUpdated = true;
        }

        if (!string.IsNullOrEmpty(externalReference) && ExternalReference != externalReference)
        {
            ExternalReference = externalReference;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new ReceiptUpdated { Receipt = this });
        }

        return this;
    }
}

