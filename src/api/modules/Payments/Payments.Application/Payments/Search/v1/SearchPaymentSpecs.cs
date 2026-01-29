using Ardalis.Specification;
using FSH.Starter.WebApi.Payments.Application.Payments.Get.v1;
using FSH.Starter.WebApi.Payments.Domain;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Search.v1;

public sealed class SearchPaymentSpecs : Specification<Payment, PaymentResponse>
{
    public SearchPaymentSpecs(SearchPaymentsCommand request)
    {
        Query.OrderByDescending(p => p.PaymentDate);

        if (request.CustomerId.HasValue)
        {
            Query.Where(p => p.CustomerId == request.CustomerId.Value);
        }

        if (request.InvoiceId.HasValue)
        {
            Query.Where(p => p.InvoiceId == request.InvoiceId.Value);
        }

        if (request.Status.HasValue)
        {
            Query.Where(p => p.Status == request.Status.Value);
        }

        if (request.Method.HasValue)
        {
            Query.Where(p => p.Method == request.Method.Value);
        }

        if (request.FromDate.HasValue)
        {
            Query.Where(p => p.PaymentDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            Query.Where(p => p.PaymentDate <= request.ToDate.Value);
        }
    }
}
