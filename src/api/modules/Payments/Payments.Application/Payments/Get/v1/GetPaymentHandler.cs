using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Payments.Domain;
using FSH.Starter.WebApi.Payments.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Get.v1;

public sealed class GetPaymentHandler(
    [FromKeyedServices("payments:payments")] IReadRepository<Payment> repository)
    : IRequestHandler<GetPaymentRequest, PaymentResponse>
{
    public async Task<PaymentResponse> Handle(GetPaymentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var payment = await repository.FirstOrDefaultAsync(
            new GetPaymentSpecs(request.Id), cancellationToken);
        
        return payment ?? throw new PaymentNotFoundException(request.Id);
    }
}
