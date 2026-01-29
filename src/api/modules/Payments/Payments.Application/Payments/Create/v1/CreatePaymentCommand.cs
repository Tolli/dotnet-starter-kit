using System.ComponentModel;
using FSH.Starter.WebApi.Payments.Domain;
using MediatR;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Create.v1;

public sealed record CreatePaymentCommand(
    [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid CustomerId,
    [property: DefaultValue(null)] Guid? InvoiceId,
    [property: DefaultValue(100.00)] decimal Amount,
    [property: DefaultValue("ISK")] string Currency,
    [property: DefaultValue(PaymentMethod.BankTransfer)] PaymentMethod Method,
    [property: DefaultValue("Payment description")] string? Description = null) : IRequest<CreatePaymentResponse>;
