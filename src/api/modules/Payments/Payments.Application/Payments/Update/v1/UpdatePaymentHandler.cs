using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Payments.Domain;
using FSH.Starter.WebApi.Payments.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Update.v1;

public sealed class UpdatePaymentHandler(
    ILogger<UpdatePaymentHandler> logger,
    [FromKeyedServices("payments:payments")] IRepository<Payment> repository)
    : IRequestHandler<UpdatePaymentCommand, UpdatePaymentResponse>
{
    public async Task<UpdatePaymentResponse> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var payment = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new PaymentNotFoundException(request.Id);

        payment.Update(request.Amount, request.Description, request.Currency);
        
        await repository.UpdateAsync(payment, cancellationToken);
        logger.LogInformation("payment updated {PaymentId}", payment.Id);
        
        return new UpdatePaymentResponse(payment.Id);
    }
}
