using FSH.Starter.WebApi.Payments.Domain;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Get.v1;

public sealed record PaymentResponse
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public Guid? InvoiceId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public PaymentStatus Status { get; init; }
    public PaymentMethod Method { get; init; }
    public string? TransactionId { get; init; }
    public string? Description { get; init; }
    public DateTime PaymentDate { get; init; }
    public string? GatewayResponse { get; init; }
    public string? FailureReason { get; init; }
    public DateTime? CreatedOn { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? LastModifiedOn { get; init; }
    public Guid? LastModifiedBy { get; init; }
}
