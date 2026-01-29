using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Payments.Domain.Events;

namespace FSH.Starter.WebApi.Payments.Domain;

public class Payment : AuditableEntity, IAggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "ISK";
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? TransactionId { get; private set; }
    public string? Description { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public string? GatewayResponse { get; private set; }
    public string? FailureReason { get; private set; }

    private Payment() { }

    private Payment(
        Guid id,
        Guid customerId,
        Guid? invoiceId,
        decimal amount,
        string currency,
        PaymentMethod method,
        string? description)
    {
        Id = id;
        CustomerId = customerId;
        InvoiceId = invoiceId;
        Amount = amount;
        Currency = currency;
        Method = method;
        Description = description;
        Status = PaymentStatus.Pending;
        PaymentDate = DateTime.UtcNow;

        QueueDomainEvent(new PaymentCreated { Payment = this });
    }

    public static Payment Create(
        Guid customerId,
        Guid? invoiceId,
        decimal amount,
        string currency,
        PaymentMethod method,
        string? description)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));

        return new Payment(Guid.NewGuid(), customerId, invoiceId, amount, currency, method, description);
    }

    public Payment MarkAsCompleted(string transactionId, string? gatewayResponse = null)
    {
        if (Status == PaymentStatus.Completed)
            return this;

        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        GatewayResponse = gatewayResponse;
        FailureReason = null;

        QueueDomainEvent(new PaymentCompleted { Payment = this });
        return this;
    }

    public Payment MarkAsFailed(string failureReason, string? gatewayResponse = null)
    {
        if (Status == PaymentStatus.Failed)
            return this;

        Status = PaymentStatus.Failed;
        FailureReason = failureReason;
        GatewayResponse = gatewayResponse;

        QueueDomainEvent(new PaymentFailed { Payment = this });
        return this;
    }

    public Payment MarkAsCancelled(string reason)
    {
        if (Status == PaymentStatus.Cancelled)
            return this;

        Status = PaymentStatus.Cancelled;
        FailureReason = reason;

        QueueDomainEvent(new PaymentCancelled { Payment = this });
        return this;
    }

    public Payment Update(decimal? amount, string? description, string? currency)
    {
        bool isUpdated = false;

        if (amount.HasValue && amount.Value != Amount && amount.Value > 0)
        {
            Amount = amount.Value;
            isUpdated = true;
        }

        if (!string.IsNullOrWhiteSpace(description) && Description != description)
        {
            Description = description;
            isUpdated = true;
        }

        if (!string.IsNullOrWhiteSpace(currency) && Currency != currency)
        {
            Currency = currency;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new PaymentUpdated { Payment = this });
        }

        return this;
    }
}

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    Refunded = 5
}

public enum PaymentMethod
{
    CreditCard = 0,
    DebitCard = 1,
    BankTransfer = 2,
    Cash = 3,
    PayPal = 4,
    Stripe = 5,
    Other = 99
}
