using Ardalis.Specification;
using FSH.Starter.WebApi.Payments.Domain;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Get.v1;

public sealed class GetPaymentSpecs : Specification<Payment, PaymentResponse>
{
    public GetPaymentSpecs(Guid id)
    {
        Query.Where(p => p.Id == id);
    }
}
