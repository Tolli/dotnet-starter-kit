using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Payments.Domain;
using FSH.Starter.WebApi.Payments.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Delete.v1;

public sealed class DeletePaymentHandler(
    ILogger<DeletePaymentHandler> logger,
    [FromKeyedServices("payments:payments")] IRepository<Payment> repository)
    : IRequestHandler<DeletePaymentCommand>
{
    public async Task Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var payment = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new PaymentNotFoundException(request.Id);

        await repository.DeleteAsync(payment, cancellationToken);
        logger.LogInformation("payment deleted {PaymentId}", request.Id);
    }
}
