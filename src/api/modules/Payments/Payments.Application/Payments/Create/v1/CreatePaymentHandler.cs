using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Payments.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Create.v1;

public sealed class CreatePaymentHandler(
    ILogger<CreatePaymentHandler> logger,
    [FromKeyedServices("payments:payments")] IRepository<Payment> repository)
    : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
{
    public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var payment = Payment.Create(
            request.CustomerId,
            request.InvoiceId,
            request.Amount,
            request.Currency,
            request.Method,
            request.Description);

        await repository.AddAsync(payment, cancellationToken);
        logger.LogInformation("payment created {PaymentId}", payment.Id);
        
        return new CreatePaymentResponse(payment.Id);
    }
}
