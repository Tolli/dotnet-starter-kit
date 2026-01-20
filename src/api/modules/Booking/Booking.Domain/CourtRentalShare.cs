using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public class CourtRentalShare : AuditableEntity, IAggregateRoot
{
    public decimal AmountTotal { get; private set; }
    public decimal AmountPaid { get; private set; }
    public decimal Discount { get; private set; }
    public string? ExternalReference { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime PaidDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public Guid CourtRentalId { get; private set; }
    public Guid CustomerId { get; private set; }
    public virtual CourtRental CourtRental { get; private set; } = default!;
    public virtual Customer Customer {  get; private set; } = default!;
    private CourtRentalShare(Guid id, Guid courtRentalId, Guid customerId, decimal amountTotal, decimal amountPaid, decimal discount, DateTime paidDate, DateTime dueDate, string? externalReference)
    {
        Id = id;
        CourtRentalId = courtRentalId;
        CustomerId = customerId;
        AmountTotal = amountTotal;
        AmountPaid = amountPaid;
        Discount = discount;
        PaidDate = paidDate;
        DueDate = dueDate;
        IsPaid = amountPaid >= amountTotal;
        ExternalReference = externalReference;

        QueueDomainEvent(new CourtRentalShareCreated { CourtRentalShare = this });
    }

    public static CourtRentalShare Create(Guid courtRentalId, Guid customerId, decimal amountTotal, decimal amountPaid, decimal discount, DateTime paidDate, DateTime dueDate, string? externalReference)
    {
        return new CourtRentalShare(Guid.NewGuid(), courtRentalId, customerId, amountTotal, amountPaid, discount, paidDate, dueDate, externalReference);
    }

    public CourtRentalShare Update(Guid? courtRentalId, Guid? customerId, decimal? amountTotal, decimal? amountPaid, decimal? discount, DateTime? paidDate, DateTime? dueDate, string? externalReference)
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

        if (courtRentalId.HasValue && CourtRentalId != courtRentalId)
        {
            CourtRentalId = courtRentalId.Value;
            isUpdated = true;
        }

        if (customerId.HasValue && CustomerId != customerId)
        {
            CustomerId = customerId.Value;
            isUpdated = true;
        }
        
        if (paidDate.HasValue && PaidDate != paidDate)
        {
            PaidDate = paidDate.Value;
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
            QueueDomainEvent(new CourtRentalShareUpdated { CourtRentalShare = this });
        }

        return this;
    }
}

